using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace HttpFunctionAppAuth0Play.Security
{
    public class AuthorizationResult
    {
        public AuthorizationResult(ClaimsPrincipal claimsPrincipal, SecurityToken securityToken)
        {
            ClaimsPrincipal = claimsPrincipal;
            SecurityToken = securityToken;
        }

        public AuthorizationResult(string failureReason)
        {
            FailureReason = failureReason;
        }

        public ClaimsPrincipal ClaimsPrincipal { get; }

        public string FailureReason { get; }

        public SecurityToken SecurityToken { get; }

        public bool Success => ClaimsPrincipal != null && string.IsNullOrEmpty(FailureReason);
    }
}
