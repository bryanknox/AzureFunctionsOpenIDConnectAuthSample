using System;
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

        public bool GetIssuerSigningKeysAsyncShouldThrow { get; set; }

        public IEnumerable<SecurityKey> SecurityKeysForTest { get; set; }


        // IOidcConfigurationManager members

        public async Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync()
        {
            // Prevent compiler Warning CS1998 "This async method lacks 'await' operators and ..."
            await Task.FromResult(0);

            if (GetIssuerSigningKeysAsyncShouldThrow)
            {
                throw new TestException(ExceptionMessageForTest);
            }
            return SecurityKeysForTest;
        }

        public void RequestRefresh()
        {
            throw new NotImplementedException();
        }
    }
}
