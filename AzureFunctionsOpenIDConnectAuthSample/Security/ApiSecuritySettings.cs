namespace AzureFunctionsOpenIDConnectAuthSample.Security
{
    /// <summary>
    /// Encapsulates settings used in API authorization.
    /// </summary>
    public class ApiSecuritySettings
    {
        /// <summary>
        /// The identifies the API to be authroized to the authorization service.
        /// For Autho0 this is the API's identifier in the Auth0 Dashboard. 
        /// </summary>
        public string AuthorizationAudience { get; set; }

        /// <summary>
        /// The URL of the service that will perform API authorization.
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </summary>
        public string AuthorizationIssuerUrl { get; set; }
    }
}
