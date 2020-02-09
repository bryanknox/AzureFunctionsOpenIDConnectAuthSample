using System.Threading;
using System.Threading.Tasks;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
{
    public class OidcConfigurationManager : IOidcConfigurationManager
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

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
        /// Returns the cached configuration if it was retrieved previously. If it hasn't been retrieved
        /// or the cached configuration is stale, then a fresh OpenIdConnectConfiguration is retrieved
        /// from the OpenID Connect provider, cached, and returned.
        /// </summary>
        /// <returns>
        /// The current OpenIdConnectConfiguration.
        /// </returns>
        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync()
        {
            return await _configurationManager.GetConfigurationAsync(CancellationToken.None);
        }

        /// <summary>
        /// Requests that the next call to GetConfigurationAsync() obtain new configuration.
        /// If the last refresh was greater than RefreshInterval then the next call to
        /// GetConfigurationAsync() will retrieve new configuration.
        /// If RefreshInterval == System.TimeSpan.MaxValue then this method does nothing.
        /// </summary>
        public void RequestRefresh()
        {
            _configurationManager.RequestRefresh();
        }
    }
}
