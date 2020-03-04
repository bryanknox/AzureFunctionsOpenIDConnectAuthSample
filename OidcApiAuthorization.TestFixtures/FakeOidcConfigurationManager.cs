using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeOidcConfigurationManager : IOidcConfigurationManager
    {
        public Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync()
        {
            throw new NotImplementedException();
        }

        public void RequestRefresh()
        {
            throw new NotImplementedException();
        }
    }
}
