# AzureFunctionsOpenIDConnectAuthSample
A sample app that shows how to protect access to an API that is implemented as an HTTP triggered Azure Function and where the authorization server used supports OpenID Connect (OIDC) protocols.

Service providers that support compatible authorization servers include [Auth0](https://auth0.com/), [okta](https://okta.com/) and many others.

# This Sample Uses:
- C#
- .NET Core v3.1
- Azure Functions v3
- [Dependency Injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
- [Options Pattern for App Settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings)
- OpenIDConnect (OIDC), Bearer tokens (OAuth 2.0 JSON Web Tokens (JWT))

# The Sample Solution Includes:
- Unit tests using [xUnit.net](https://xunit.net/)
- CI using GitHub Actions

# Introduction
I implemented a sample serverless API as an HTTP triggered Azure Function and enhanced it to work within an OAuth 2.0 Client Credentials Flow to ensure that only authorized apps that use OpenID Connect (OIDC) can access that API.

What follows is my high-level description of the concepts and API-side implementation of the mechanisms used to protect an API implemented as an HTTP triggered Azure Function using the OAuth 2.0 Client Credentials Flow.

Here's the breakdown of what follows:
1. Some context, a high-level description of the OIDC and OAuth 2.0 flows used.
1. How to make a call to a protected API.
1. How the code that protects the API is called within an HTTP triggered Azure Function.
1. The implementation of the sample code that protects the API.

# Context - OIDC and OAuth 2.0 Flows Used
The techniques used in the sample code can be used for protecting other HTTP triggered Azure Functions. If those the authorization server supports an OpenID Connect (OIDC) `/.well-known/openid-configuration` endpoint that the Azure Function will use to get the information it needs to authorize calls to the API.

Before the client application makes calls to the protected API it will typically go through a flow similar to the following:
1. The client application sends its Client ID and Client Secret to the authorization server.
1. The authorization server returns a JSON Web Token (JWT) containing an Access Token.
1. The client application includes that JWT in subsequent calls to the protected API as an `Authorization` header, where the value of that header is a Bearer token containing the JWT.
 
That flow that the client applications used for calling protected APIs in that way is called the OAuth 2.0 Client Credentials Flow. See [OAuth 2.0 RFC 6749, section 4.4 Client Credentials Grant](https://tools.ietf.org/html/rfc6749#section-4.4).

Its important to note that in the Client Credentials Flow access to the API is granted to the client application, not a user of the client application user.

I won't be going into more details of how the OAuth 2.0 Client Credentials Flow works or how it can be implemented within a client app. Instead I'll focus on describing the API-side of how the call to the Azure Function is protected.

## ODIC Authorization Server and Services
There are many authentication and authorization services the support OpenID Connect (OIDC) and the OAuth 2.0 Client Credentials Flow.  I've used [Auth0](https://auth0.com/) to host my authorization service in the development and testing of the sample API. However there is no [Auth0](https://auth0.com/) specific dependencies or code in the sample. The sample code is based on the open web standards OpenID Connect (OIDC), OAuth 2.0 and JSON Web Tokens (JWT). The sample code should work with authorization service providers that follow those standards, hopefully with only changes to app settings.

# Calling a protected API

## Authorization header with JWT
All calls to APIs that use the OAuth 2.0 Client Credentials Flow must include an `Authorization` header for the JWT containing the Access Token. That request header will have the name `Authorization` (case sensitive), and a value of `Bearer ` (note the trailing space character) followed by the JWT.

`Bearer JWT_ACCESS_TOKEN`

## Sending Requests to the HelloFunction Sample API
The sample HelloFunction API implements simple hello-world functionality. A name is given in the request and the response will say hello to that name.

The request must be an HTTP POST that includes the required `Authorization` header, plus a `content-type` header with the value `application/json` , and a JSON body with the following format.

{"name": "a name goes here"}

### Example cURL for calling the API
The following cURL command shows the format for calling the HelloFunction Sample API.

curl --request POST \
  --url https://sampleapi.com/api/HelloFunction \
  --header 'Authorization: Bearer JWT_ACCESS_TOKEN' \
  --header 'content-type: application/json' \
  --data '{"name":"world"}'

### Example Response Body:

Hello, world

# Protecting an HTTP triggered Azure Function 

In the sample code the sample HTTP triggered Azure Function is implemented in the  `SampleFunctionApp` project's `HelloFunction`.

The function itself is pretty simple.
```
[FunctionName(nameof(HelloFunction))]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
{
    log.LogWarning("C# HTTP trigger function received a request.");

    ApiAuthorizationResult authorizationResult = await _apiAuthorization.AuthorizeAsync(req.Headers);
    if (authorizationResult.Failed)
    {
        log.LogWarning(authorizationResult.FailureReason);
        return new UnauthorizedResult();
    }
    log.LogWarning("C# HTTP trigger function rquest is authorized.");

    // Get name from request body.
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    string name = data?.name;

    return !string.IsNullOrWhiteSpace(name)
        ? (ActionResult)new OkObjectResult($"Hello, {name}")
        : new BadRequestObjectResult("Please pass a name the request body.");
}

```
Here's the code in that function that protects the API.
```
    ApiAuthorizationResult authorizationResult = await _apiAuthorization.AuthorizeAsync(req.Headers);
    if (authorizationResult.Failed)
    {
        log.LogWarning(authorizationResult.FailureReason);
        return new UnauthorizedResult();
    }
```

The `AuthorizeAsync(..)` method is passed the headers from the HttpRequest and it returns an `ApiAuthorizationResult`. If the results indicates failure then an UnauthorizedResult (HTTP 401) is returned to the caller of the API.

The sample code uses dependency injection, so an object that implements the `IApiAuthorization` interface is injected in the functions constructor. The OidcApiAuthorizationService is the class in the sample code that implements that interface. Its located in the `OidcApiAuthorization` project.

# Overview of the Sample Code Projects
The sample code is composed of a Visual Studio solution with multiple projects.

With all the projects and files the code might seem complex but each piece is pretty simple and easy to understand.

**SampleFunctionApp** - is where you'll find the sample HTTP triggered Azure Function.

**SampleFunctionApp.Tests** - is where you'll find the unit tests for the sample Azure Function.

**TestFixtures.AzureFunctions** - is a class library with some test fixtures that make implementing the unit tests easier.

**OidcApiAuthorization** - is the class library for the code that protects APIs. `OidcApiAuthorizationService` is the top-level class. It uses the other code that library which is broken down into multiple interfaces and classes so its easier to unit test. The number of files may make it seem complex, but each piece should be easy to understand and hopefully its pretty easy to see how they fit together.

**OidcApiAuthorization.TestFixtures** - is a class library of test fixtures that make unit testing the class in the OidcApiAuthorization library.

**OidcApiAuthorization.Tests** - contains the unit tests for the OidcApiAuthorization library.

# OidcApiAuthorizationService class
This is the top-level class used to protect HTTP triggered Azure Function APIs.

## AuthorizeAsync(..) method
The `AuthorizeAsync(..)` method is passed the headers from an HttpRequest and it returns an `ApiAuthorizationResult`. 

There are three steps involved in authorizing a request.
1. Get the JWT containing the Access Token from the requests `Authorization` header.
2. Get the issuer's signing keys.
3. Validate the Access Token using the issuer's signing keys.

### Get the JWT from the Authorization header
The `AuthorizationHeaderBearerTokenExtractor` class handles finding the `Authorization` header and extracting the JWT from the header's value.

### Get the issuer's signing keys
Getting the issuer's signing keys is handled by class `OidcConfigurationManager` which is  a very thin wrapper around and instance of `ConfigurationManager<OpenIdConnectConfiguration>` from the `Microsoft.IdentityModel.Protocols` namespace.

The ConfigurationManager is configured point to the issuer (authorization server) where it can get the signing keys.

The ConfigurationManager doesn't actually call the issuer until `GetConfigurationAsync()` is called. The configuration retrieved is cached, and cached version is returned on subsequent calls to `GetConfigurationAsync()`. The ConfigurationManager will automatically call the issuer when it needs to refresh the cache.

The ConfigurationManager also has a `RequestRefresh()` method that is called when we need to force a refresh of the signing keys the next time `GetConfigurationAsync()` is called. We'll get into that more in the Retry section below.

#### Reducing calls to the issuer
The use of cached signing keys is very important because it reduces the number of calls to the issuer. Besides performance it still a good idea to minimize the calls to the issuer because most commercial authorization service providers will limit or throttle the number of calls  you can make to those configuration endpoints.

Another mechanism we use to reduce calls to the issuer is to ensure that there is only one instance of `OidcApiAuthorizationService` for our Azure Function. We do this by registering it as a singleton in the function app's dependency injection configuration.  See `ServicesConfigurationExtensions.AddOidcApiAuthorization()`.  Also see the Microsoft Docs: [Use dependency injection in .NET Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection).

#### Configuring the ConfigurationManager
The ConfigurationManager needs to be configured and we want to get the setting it needs from the function app's app settings.

We configure the function app's dependency injection so that we can use the [Options Pattern for App Settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings).  Then we inject the settings we need into the `OidcApiAuthorizationService` constructor. That constructor passes the issuer URL setting to the `OidcConfigurationManagerFactory` which creates  ConfigurationManager object that is used.

### Validate the Access Token using the issuer's signing keys
The validation of the access token using the issuer's signing keys is handled by the `JwtSecurityTokenHandlerWrapper` class. That class is just a very thin wrapper around an instance of class `JwtSecurityTokenHandler` from the  `System.IdentityModel.Tokens.Jwt` namespace.

`JwtSecurityTokenHandler.ValidateToken(..)` throws an exception if the token could not be validated. 

### Retry getting the signing keys and validating the token
The `JwtSecurityTokenHandler.ValidateToken(..)` throws a `SecurityTokenSignatureKeyNotFoundException` if the signing keys for validating JWT could not be found. This could happen if the issuer has changed the signing keys since the last time they were retrieved by the ConfigurationManager. To handle this we ask the ConfigurationManger to refresh by calling its `RequestRefresh()` method which causes it to retrieve the keys again the next time we ask for them. Then we retry by asking for the signing keys and validating the token again.

