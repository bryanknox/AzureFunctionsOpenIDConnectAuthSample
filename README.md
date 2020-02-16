# AzureFunctionsOpenIDConnectAuthSample
Sample Azure Functions app that shows the use of OpenID Connect (OIDC) to protect access to an HTTP triggered Azure Function. 

OIDC uses OAuth 2.0 JSON Web Tokens (JWT) (Bearer tokens) as part of the mechanism for API authorization security.

# This Sample Uses:
- Azure Functions v3
- .NET Core v3.1
- [Dependency Injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
- [Options Pattern for App Settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings)
- OpenIDConnect (OIDC), Bearer tokens (OAuth 2.0 JSON Web Tokens (JWT))
- NuGet packages:
  - Microsoft.Azure.Functions.Extensions
  - Microsoft.IdentityModel.Protocols
  - Microsoft.IdentityModel.Protocols.OpenIdConnect
  - Microsoft.NET.Sdk.Functions
  - System.IdentityModel.Tokens.Jwt
