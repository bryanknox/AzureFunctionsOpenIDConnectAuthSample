using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeSecurityToken : SecurityToken
    {
        public override string Id => throw new NotImplementedException();

        public override string Issuer => throw new NotImplementedException();

        public override SecurityKey SecurityKey => throw new NotImplementedException();

        public override SecurityKey SigningKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override DateTime ValidFrom => throw new NotImplementedException();

        public override DateTime ValidTo => throw new NotImplementedException();
    }
}
