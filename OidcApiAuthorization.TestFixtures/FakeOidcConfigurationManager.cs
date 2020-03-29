using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;
using TestFixtures.AzureFunctions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeOidcConfigurationManager : IOidcConfigurationManager
    {
        public string ExceptionMessageForTest { get; set; }

        public int GetIssuerSigningKeysAsyncCalledCount { get; set; }

        public int RequestRefreshCalledCount { get; set; }

        public IEnumerable<SecurityKey> SecurityKeysForTest { get; set; }

        // IOidcConfigurationManager members

        public async Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync()
        {
            ++GetIssuerSigningKeysAsyncCalledCount;

            if (ExceptionMessageForTest != null)
            {
                throw new TestException(ExceptionMessageForTest);
            }
            return await Task.FromResult(SecurityKeysForTest);
        }

        public void RequestRefresh()
        {
            ++RequestRefreshCalledCount;
        }
    }
}
