using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace OidcApiSecurity.Abstractions
{
    public interface IJwtSecurityTokenHandlerWrapper
    {
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters, out SecurityToken securityToken);
    }
}
