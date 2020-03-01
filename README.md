# AzureFunctionsOpenIDConnectAuthSample
Sample Azure Functions app that shows the use of OpenID Connect (OIDC) to protect access to an HTTP triggered Azure Function. 

OIDC uses OAuth 2.0 JSON Web Tokens (JWT) (Bearer tokens) as part of the mechanism for API authorization security.

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
