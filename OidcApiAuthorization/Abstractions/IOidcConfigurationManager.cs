using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace OidcApiAuthorization.Abstractions
{
    public interface IOidcConfigurationManager
    {
        /// <summary>
        /// Returns the cached signing keys if they were retrieved previously.
        /// If they haven't been retrieved, or the cached keys are stale, then a fresh set of
        /// signing keys are retrieved from the OpenID Connect provider (issuer) cached and returned.
        /// This method will throw if the configuration cannot be retrieved, instead of returning null.
        /// </summary>
        /// <returns>
        /// The current set of the Open ID Connect issuer's signing keys.
        /// </returns>
        Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync();

        /// <summary>
        /// Requests that the next call to GetConfigurationAsync() obtain new configuration.
        /// </summary>
        void RequestRefresh();
    }
}
