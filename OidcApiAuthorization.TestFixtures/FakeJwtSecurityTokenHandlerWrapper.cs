using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeJwtSecurityTokenHandlerWrapper : IJwtSecurityTokenHandlerWrapper
    {
        public ClaimsPrincipal ClaimsPrincipalToReturn { get; set; }

        public SecurityToken SecurityTokenToOutput { get; set; }


        // IJwtSecurityTokenHandlerWrapper members

        public ClaimsPrincipal ValidateToken(
            string token,
            TokenValidationParameters tokenValidationParameters,
            out SecurityToken securityToken)
        {
            securityToken = SecurityTokenToOutput;

            return ClaimsPrincipalToReturn;
        }
    }
}
