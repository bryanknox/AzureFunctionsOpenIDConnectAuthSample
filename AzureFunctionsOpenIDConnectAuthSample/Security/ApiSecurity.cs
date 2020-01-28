using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SampleApp.Security.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SampleApp.Security
{
    public class ApiSecurity : IApiSecurity
    {
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        private readonly string _issuerUrl = null;
        private readonly string _audience = null;

        public ApiSecurity(IOptions<ApiSecuritySettings> apiSecuritySettingsOptions)
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
        }

        public async Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log)
        {
            string authorizationToken = GetAuthorizationTokenFromHeaders(httpRequestHeaders);
            if (authorizationToken == null)
            {
                return new AuthorizationResult("Authorization header missing or is not a Bearer token.");
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

                    var handler = new JwtSecurityTokenHandler();
                    claimsPrincipal = handler.ValidateToken(
                        authorizationToken,
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

        /// <summary>
        /// Gets the JWT Bearer token from the given HTTP request headers.
        /// </summary>
        /// <param name="headers">
        /// The headers from the HTTP request.
        /// </param>
        /// <returns>
        /// The JWT Bearer token parsed from the Authorization header, or nul if the uthorization header was not found
        /// or its value is not a Bearer token.
        /// </returns>
        private static string GetAuthorizationTokenFromHeaders(IHeaderDictionary headers)
        {
            // Get a StringValues object that represents the content of the Authorization header found in the given
            // headers. If the Authorization header is not found the StringValues.Value returned will be nul.
            // Default for a KeyValuePair<string, StringValues> has a Value that is a StringValue with a null string.
            string rawAuthorizationHeaderValueString = headers.SingleOrDefault(x => x.Key == "Authorization").Value;

            AuthenticationHeaderValue authenticationHeaderValue = AuthenticationHeaderValue.Parse(rawAuthorizationHeaderValueString);

            if (authenticationHeaderValue == null 
                || !string.Equals(authenticationHeaderValue.Scheme, "Bearer", StringComparison.InvariantCultureIgnoreCase))
            {
                // The Authorization header was not found, or its value was not a Bearer token.
                return null;
            }

            // Return the token parsed from the Athorization header.
            return authenticationHeaderValue.Parameter;
        }
    }
}
