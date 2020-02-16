using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization
{
    public class JwtSecurityTokenHandlerWrapper : IJwtSecurityTokenHandlerWrapper
    {
        public ClaimsPrincipal ValidateToken(
            string token,
            TokenValidationParameters tokenValidationParameters,
            out SecurityToken securityToken)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(
                token,
                tokenValidationParameters,
                out securityToken);

            return claimsPrincipal;
        }
    }
}
