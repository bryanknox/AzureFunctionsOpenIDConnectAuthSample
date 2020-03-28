using System;
using Microsoft.Extensions.Options;

namespace OidcApiAuthorization
{
    public class OidcApiAuthorizationHealthCheck
    {
        private readonly OidcApiAuthorizationSettings _oidcApiAuthorizationSettings;

        public OidcApiAuthorizationHealthCheck(
            IOptions<OidcApiAuthorizationSettings> settingsOptions)
        {
            _oidcApiAuthorizationSettings = settingsOptions?.Value;
        }

        /// <summary>
        /// Throws an Exception if any of the setting properties have not been set.
        /// </summary>
        public void ThrowIfMissingSettings()
        {
            if (_oidcApiAuthorizationSettings == null
                || string.IsNullOrWhiteSpace(_oidcApiAuthorizationSettings.Audience)
                || string.IsNullOrWhiteSpace(_oidcApiAuthorizationSettings.IssuerUrl))
            {
                throw new Exception(
                    $"Some or all {nameof(OidcApiAuthorizationSettings)} are missing.");
            }
        }

    }
}
