using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions
{
    public interface IOidcConfigurationManager
    {
        /// <summary>
        /// Retrieve the current OpenIdConnectConfiguration, refreshing and/or caching as needed.
        /// This method will throw if the configuration cannot be retrieved, instead of returning null.
        /// </summary>
        /// <returns>
        /// The current OpenIdConnectConfiguration.
        /// </returns>
        Task<OpenIdConnectConfiguration> GetConfigurationAsync();

        /// <summary>
        /// Requests that the next call to GetConfigurationAsync() obtain new configuration.
        /// </summary>
        void RequestRefresh();
    }
}
