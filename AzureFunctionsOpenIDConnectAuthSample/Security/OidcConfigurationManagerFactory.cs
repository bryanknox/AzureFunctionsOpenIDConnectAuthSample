using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
{
    public class OidcConfigurationManagerFactory : IOidcConfigurationManagerFactory
    {
        /// <summary>
        /// Construct a new IOidcConfigurationManager
        /// </summary>
        /// <param name="issuerUrl">
        /// The URL of the Open ID Connect provider (issuer).
        /// </param>
        /// <returns>
        /// The new IOidcConfigurationManager.
        /// </returns>
        public IOidcConfigurationManager New(string issuerUrl)
        {
            return new OidcConfigurationManager(issuerUrl);
        }
    }
}
