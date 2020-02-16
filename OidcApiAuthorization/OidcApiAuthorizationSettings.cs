using System;

namespace OidcApiAuthorization
{
    /// <summary>
    /// Encapsulates settings used in OpenID Connect (OIDC) API authorization.
    /// </summary>
    public class OidcApiAuthorizationSettings
    {
        private string _issuerUrl;

        /// <summary>
        /// Identifies the API to be authorized by the Open ID Connect provider (issuer).
        /// </summary>
        /// <remarks>
        /// The "Audiance" is the indentifer used by the authorization provider to indentify
        /// the API (HTTP triggered Azure Function) being protected. This is often a URL but
        /// it is not used as a URL is is simply used as an identifier.
        /// 
        /// For Auth0 the AuthorizationAudience setting set here should match the API's Identifier
        /// in the Auth0 Dashboard.
        /// </remarks>
        public string AuthorizationAudience { get; set; }

        /// <summary>
        /// The URL of the Open ID Connect provider (issuer) that will perform API authorization.
        /// </summary>
        /// <remarks>
        /// The "Issuer" is the URL for the authorization provider's end-point. This URL will be
        /// used as part of the OpenID Connect protocol to obtain the the signing keys
        /// that will be used to validate the JWT Bearer tokens in incoming HTTP request headers.
        /// 
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </remarks>
        public string AuthorizationIssuerUrl {
            get
            {
                return _issuerUrl;
            }
            set
            {
                if (value != null && !value.EndsWith("/"))
                {
                    _issuerUrl = value + "/";
                }
                else
                {
                    _issuerUrl = value;
                }
            }
        }

        /// <summary>
        /// Throws an Exception if any of the setting properties have not been set.
        /// </summary>
        public void ThrowIfMissingSettings()
        {
            if (string.IsNullOrWhiteSpace(AuthorizationAudience))
            {
                throw new Exception($"Missing application setting. {nameof(OidcApiAuthorizationSettings.AuthorizationAudience)} setting is not set.");
            }
            if (string.IsNullOrWhiteSpace(AuthorizationIssuerUrl))
            {
                throw new Exception($"Missing application setting. {nameof(OidcApiAuthorizationSettings.AuthorizationIssuerUrl)} setting is not set.");
            }
        }
    }
}
