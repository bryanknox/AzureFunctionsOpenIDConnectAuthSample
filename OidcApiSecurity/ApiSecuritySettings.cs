using System;

namespace OidcApiSecurity
{
    /// <summary>
    /// Encapsulates settings used in API authorization.
    /// </summary>
    public class ApiSecuritySettings
    {
        private string _issuerUrl;

        /// <summary>
        /// Identifies the API to be authorized by the Open ID Connect provider (issuer).
        /// For Autho0 this is the API's identifier in the Auth0 Dashboard. 
        /// </summary>
        /// <remarks>
        /// The "Audiance" is the indentifer used by the authorization provider to indentify
        /// the API (HTTP triggered Azure Function) being protected. This is often a URL but
        /// For Auth0 the AuthorizationAudience setting set here should match the API's Identifier
        /// in the Auth0 Dashboard.
        /// </remarks>
        public string AuthorizationAudience { get; set; }

        /// <summary>
        /// The URL of the Open ID Connect provider (issuer) that will perform API authorization.
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </summary>
        /// <remarks>
        /// The "Issuer" is the URL for the authorization provider's end-point.
        /// This URL will be used as part of the OpenID Connect protocol to obtain the the signing keys
        /// that will be used to validate the JWT Bearer tokens in incoming HTTP request headers.
        /// </remarks>
        public string AuthorizationIssuerUrl {
            get
            {
                if (_issuerUrl != null && !_issuerUrl.EndsWith("/"))
                {
                    return _issuerUrl + "/";
                }
                return _issuerUrl;
            }
            set
            {
                _issuerUrl = value;
            }
        }

        /// <summary>
        /// Throws an Exception if any of the setting properties values are invalid.
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (string.IsNullOrWhiteSpace(AuthorizationAudience))
            {
                throw new Exception($"Missing application setting. {nameof(ApiSecuritySettings.AuthorizationAudience)} setting is not set.");
            }
            if (string.IsNullOrWhiteSpace(AuthorizationIssuerUrl))
            {
                throw new Exception($"Missing application setting. {nameof(ApiSecuritySettings.AuthorizationIssuerUrl)} setting is not set.");
            }
        }
    }
}
