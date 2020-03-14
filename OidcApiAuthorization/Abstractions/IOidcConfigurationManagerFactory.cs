namespace OidcApiAuthorization.Abstractions
{
    public interface IOidcConfigurationManagerFactory
    {
        /// <summary>
        /// Create a new IOidcConfigurationManager instance.
        /// </summary>
        /// <param name="issuerUrl">
        /// The URL of the Open ID Connect provider (issuer).
        /// </param>
        /// <returns>
        /// The new IOidcConfigurationManager.
        /// </returns>
        IOidcConfigurationManager Create(string issuerUrl);
    }
}
