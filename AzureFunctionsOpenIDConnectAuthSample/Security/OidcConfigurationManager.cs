using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
{
    public class OidcConfigurationManager : IOidcConfigurationManager
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        /// <summary>
        /// Construct a ConfigurationManager instance for retreiving and caching OpenIdConnectConfiguration
        /// from an Open ID Connect provider (issuer)
        /// </summary>
        /// <param name="issuerUrl">
        /// The URL of the Open ID Connect provider (issuer).
        /// Must end with a '/'.
        /// </param>
        public OidcConfigurationManager(string issuerUrl)
        {
            var documentRetriever = new HttpDocumentRetriever { RequireHttps = issuerUrl.StartsWith("https://") };

            // Setup the ConfigurationManager to call the issuer (i.e. Auth0) of the signing keys.
            // The ConfigurationManager caches the configuration it receives from the OpenID Connect provider (issuer)
            // in order to reduce the number or requests to that provider.
            //
            // The configuration is not retrieved from the OpenID Connect provider until the first time
            // the ConfigurationManager.GetConfigurationAsync() is called below.
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuerUrl}.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        /// <summary>
        /// Returns the cached signing keys if they were retrieved previously.
        /// If they haven't been retrieved, or the cached keys are stale, then a fresh set of
        /// signing keys are retrieved from the OpenID Connect provider (issuer) cached and returned.
        /// This method will throw if the configuration cannot be retrieved, instead of returning null.
        /// </summary>
        /// <returns>
        /// The current set of the Open ID Connect issuer's signing keys.
        /// </returns>
        public async Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync()
        {
            OpenIdConnectConfiguration configuration = await _configurationManager.GetConfigurationAsync(
                CancellationToken.None);

            return configuration.SigningKeys;
        }

        /// <summary>
        /// Requests that the next call to GetIssuerSigningKeysAsync() obtain new signing keys.
        /// If the last refresh was greater than RefreshInterval then the next call to
        /// GetIssuerSigningKeysAsync() will retrieve new configuration (signing keys).
        /// If RefreshInterval == System.TimeSpan.MaxValue then this method does nothing.
        /// </summary>
        /// <remarks>
        /// RefreshInterval defaults to 30 seconds (00:00:30).
        /// </remarks>
        public void RequestRefresh()
        {
            _configurationManager.RequestRefresh();
        }
    }
}
