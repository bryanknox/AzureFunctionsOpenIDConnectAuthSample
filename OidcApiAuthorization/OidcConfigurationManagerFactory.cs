using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization
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
        public IOidcConfigurationManager Create(string issuerUrl)
        {
            return new OidcConfigurationManager(issuerUrl);
        }
    }
}
