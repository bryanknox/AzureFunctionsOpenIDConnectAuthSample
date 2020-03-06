using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization
{
    /// <summary>
    /// Encapsulates checks of OpenID Connect (OIDC) Authorization tokens in HTTP request headers.
    /// </summary>
    public class OidcApiAuthorizationService : IApiAuthorization
    {
        private readonly IAuthorizationHeaderBearerTokenParser _authorizationHeaderBearerTokenParser;

        private readonly IJwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandlerWrapper;

        private readonly IOidcConfigurationManager _oidcConfigurationManager;

        private readonly string _issuerUrl = null;
        private readonly string _audience = null;

        public OidcApiAuthorizationService(
            IOptions<OidcApiAuthorizationSettings> apiAuthorizationSettingsOptions,
            IAuthorizationHeaderBearerTokenParser authorizationHeaderBearerTokenParser,
            IJwtSecurityTokenHandlerWrapper jwtSecurityTokenHandlerWrapper,
            IOidcConfigurationManagerFactory oidcConfigurationManagerFactory)
        {
            apiAuthorizationSettingsOptions.Value.ThrowIfMissingSettings();

            _issuerUrl = apiAuthorizationSettingsOptions.Value.AuthorizationIssuerUrl;
            _audience = apiAuthorizationSettingsOptions.Value.AuthorizationAudience;

            _authorizationHeaderBearerTokenParser = authorizationHeaderBearerTokenParser;

            _jwtSecurityTokenHandlerWrapper = jwtSecurityTokenHandlerWrapper;

            _oidcConfigurationManager = oidcConfigurationManagerFactory.New(_issuerUrl);
        }

        /// <summary>
        /// Checks the given HTTP request headers for a valid OpenID Connect (OIDC) Authorization token.
        /// </summary>
        /// <param name="httpRequestHeaders">
        /// The HTTP request headers to check.
        /// </param>
        /// <returns>
        /// Informatoin about the success or failure of the authorization.
        /// </returns>
        public async Task<ApiAuthorizationResult> AuthorizeAsync(
            IHeaderDictionary httpRequestHeaders)
        {
            string authorizationBearerToken = _authorizationHeaderBearerTokenParser.ParseToken(
                httpRequestHeaders);
            if (authorizationBearerToken == null)
            {
                return new ApiAuthorizationResult(
                    "Authorization header is missing, invalid format, or is not a Bearer token.");
            }

            ClaimsPrincipal claimsPrincipal = null;
            SecurityToken securityToken = null;

            int validationRetryCount = 0;

            do
            {
                IEnumerable<SecurityKey> isserSigningKeys;
                try
                {
                    // Get the cached signing keys if they were retrieved previously. 
                    // If they haven't been retrieved, or the cached keys are stale,
                    // then a fresh set of signing keys are retrieved from the OpenID Connect provider
                    // (issuer) cached and returned.
                    // This method will throw if the configuration cannot be retrieved, instead of returning null.
                    isserSigningKeys = await _oidcConfigurationManager.GetIssuerSigningKeysAsync();
                }
                catch (Exception ex)
                {
                    return new ApiAuthorizationResult(
                        "Problem getting signing keys from Open ID Connect provider (issuer)."
                        + $" ConfigurationManager threw {ex.GetType()} Message: {ex.Message}");
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
                    IssuerSigningKeys = isserSigningKeys
                };

                try
                {
                    // Try to validate the token.
                    // Throws if the the token cannot be validated.
                    // If the token is successfully validiated then the ClaimsPrincipal from the JWT
                    // is returned.
                    // The ClaimsPrincipal returned does not include claims found in the JWT header.
                    claimsPrincipal = _jwtSecurityTokenHandlerWrapper.ValidateToken(
                        authorizationBearerToken,
                        tokenValidationParameters,
                        out securityToken);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // A SecurityTokenSignatureKeyNotFoundException is thrown if the signature key for
                    // validating JWT tokens could not be found. This could happen if the issuer has
                    // changed the signing keys since the last time they were retreived by the
                    // ConfigurationManager. To handle this we ask the ConfigurationManger to refresh
                    // which causes it to retreive the keys again, and then we retry the validation.
                    // We only retry once.

                    _oidcConfigurationManager.RequestRefresh();

                    validationRetryCount++;
                }
                catch (Exception ex)
                {
                    return new ApiAuthorizationResult(
                        $"Authorization Failed. {ex.GetType()} caught while validating JWT token."
                        + $"Message: {ex.Message}");
                }
            } while (claimsPrincipal == null && validationRetryCount <= 1);

            return new ApiAuthorizationResult(claimsPrincipal, securityToken);
        }
    }
}
