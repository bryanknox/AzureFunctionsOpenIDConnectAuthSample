namespace AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions
{
    public interface IOidcConfigurationManagerFactory
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
        IOidcConfigurationManager New(string issuerUrl);
    }
}
