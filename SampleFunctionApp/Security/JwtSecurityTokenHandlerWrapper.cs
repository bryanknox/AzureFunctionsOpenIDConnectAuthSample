using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
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
