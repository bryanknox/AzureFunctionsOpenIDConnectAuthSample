using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
{
    public class ApiSecurity : IApiSecurity
    {
        private readonly IAuthorizationHeaderBearerTokenParser _authorizationHeaderBearerTokenParser;
        
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        private readonly IJwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandlerWrapper;

        private readonly string _issuerUrl = null;
        private readonly string _audience = null;

        public ApiSecurity(
            IOptions<ApiSecuritySettings> apiSecuritySettingsOptions,
            IAuthorizationHeaderBearerTokenParser authorizationHeaderBearerTokenParser,
            IJwtSecurityTokenHandlerWrapper jwtSecurityTokenHandlerWrapper)
        {
            _issuerUrl = apiSecuritySettingsOptions.Value.AuthorizationIssuerUrl;
            _audience = apiSecuritySettingsOptions.Value.AuthorizationAudience;

            if (string.IsNullOrWhiteSpace(_audience))
            {
                throw new Exception("Missing application setting. 'AuthorizationAudience' setting is not set.");
            }
            if (string.IsNullOrWhiteSpace(_issuerUrl))
            {
                throw new Exception("Missing application setting. 'AuthorizationIssuerUrl' setting is not set.");
            }
            if (!_issuerUrl.StartsWith("https://"))
            {
                throw new Exception("Application setting error. 'AuthorizationIssuerUrl' setting must start with \"https://\".");
            }
            if (!_issuerUrl.EndsWith("/"))
            {
                throw new Exception("Application setting error. 'AuthorizationIssuerUrl' setting must end \"/\".");
            }

            var documentRetriever = new HttpDocumentRetriever { RequireHttps = _issuerUrl.StartsWith("https://") };

            // Setup the ConfigurationManager to call the issuer (i.e. Auth0) of the signing keys.
            // The ConfigurationManager caches the configuration it receives the OpenID provider (issuer)
            // in order to reduce the number or requests to that provider.
            //
            // The configuration is not retrieved from the OpenID provider until the first time
            // the ConfigurationManager.GetConfigurationAsync(..) is called in the Authorize(..) method below.
            //
            // The ConfigurationManager automatically refreshes the configuration after configurable timeouts. 
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{_issuerUrl}.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );

            _authorizationHeaderBearerTokenParser = authorizationHeaderBearerTokenParser;

            _jwtSecurityTokenHandlerWrapper = jwtSecurityTokenHandlerWrapper;
        }

        public async Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log)
        {
            string authorizationBearerToken = _authorizationHeaderBearerTokenParser.ParseToken(httpRequestHeaders);
            if (authorizationBearerToken == null)
            {
                return new AuthorizationResult("Authorization header is missing or is not a Bearer token.");
            }

            ClaimsPrincipal claimsPrincipal = null;
            SecurityToken securityToken = null;

            int validationRetryCount = 0;

            do
            {
                // Get the signing keys from issuer (Auth0).
                OpenIdConnectConfiguration openIdConnectConfig = null;
                try
                {
                    // Retrieve the OpenIdConnectConfig (containing the signing keys) from the OpenID provider,
                    // if they haven't been retreived since the last timeout or refresh, otherwise get the cached
                    // configuration.
                    openIdConnectConfig = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Problem getting OpenIdConnectConfiguration from OpenID provider (issuer) via ConfigurationManager.",
                        ex);
                }

                var tokenValidationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = _audience,
                    ValidateAudience = true,
                    ValidIssuer = _issuerUrl,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKeys = openIdConnectConfig.SigningKeys
                };

                try
                {
                    // Try to validate the token.
                    // Throws if the the token cannot be validated.
                    // If the token is successfully validiate then the ClaimsPrincipal from the JWT is returned.
                    // The ClaimsPrincipal returned does not include claims found in the JWT header.
                    claimsPrincipal = _jwtSecurityTokenHandlerWrapper.ValidateToken(
                        authorizationBearerToken,
                        tokenValidationParameters,
                        out securityToken);
                }
                catch (SecurityTokenSignatureKeyNotFoundException keyNotFoundException)
                {
                    // A SecurityTokenSignatureKeyNotFoundException is thrown if the signature key for validating
                    // JWT tokens could not be found. This could happen if the issuer has changed the signing keys
                    // since the last time they were retreived by the ConfigurationManager.
                    // To handle this we ask the ConfigurationManger to refresh which causes it to retreive the keys
                    // again, and then we retry the validation. We only retry once.

                    log.LogWarning(
                        keyNotFoundException,
                        "Exception validating token. JWT signature key not found."
                        + $" {(validationRetryCount == 0 ? "Retrying..." : "Refresh Retry Failed!")}.");

                    _configurationManager.RequestRefresh();

                    validationRetryCount++;
                }
                catch (SecurityTokenException tokenException)
                {
                    log.LogWarning(
                        tokenException,
                        $"Authorization Failed. Exception caught while validating token.");

                    return new AuthorizationResult("Authorization Failed.");
                }
            } while (claimsPrincipal == null && validationRetryCount <= 1);

            return new AuthorizationResult(claimsPrincipal, securityToken);
        }
    }
}
