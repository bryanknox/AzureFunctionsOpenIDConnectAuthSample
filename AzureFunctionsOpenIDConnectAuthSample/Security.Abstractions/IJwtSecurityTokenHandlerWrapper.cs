using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions
{
    public interface IJwtSecurityTokenHandlerWrapper
    {
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters, out SecurityToken securityToken);
    }
}
