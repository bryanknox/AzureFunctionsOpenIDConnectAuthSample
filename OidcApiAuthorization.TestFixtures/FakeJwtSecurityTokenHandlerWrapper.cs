using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeJwtSecurityTokenHandlerWrapper : IJwtSecurityTokenHandlerWrapper
    {
        public ClaimsPrincipal ClaimsPrincipalToReturn { get; set; }

        public SecurityToken SecurityTokenToOutput { get; set; }

        /// <summary>
        /// Indicates whether or not a SecurityTokenSignatureKeyNotFoundException
        /// should be thrown the first time that ValidateToken(..) is called.
        /// </summary>
        public bool ThrowFirstTime { get; set; }

        public Exception ExceptionToThrow { get; set; }

        public int ValidateTokenCalledCount { get; private set; }

        // IJwtSecurityTokenHandlerWrapper members

        public ClaimsPrincipal ValidateToken(
            string token,
            TokenValidationParameters tokenValidationParameters,
            out SecurityToken securityToken)
        {
            ++ValidateTokenCalledCount;
            if (ValidateTokenCalledCount == 1
                && ThrowFirstTime)
            {
                throw new SecurityTokenSignatureKeyNotFoundException();
            }

            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            securityToken = SecurityTokenToOutput;

            return ClaimsPrincipalToReturn;
        }
    }
}
