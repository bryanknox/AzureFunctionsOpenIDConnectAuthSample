using Microsoft.AspNetCore.Http;

namespace OidcApiSecurity.Abstractions
{
    public interface IAuthorizationHeaderBearerTokenParser
    {
        /// <summary>
        /// Parses the Bearer token from the Authorization header of the given HTTP request headers.
        /// </summary>
        /// <param name="headers">
        /// The headers from an HTTP request.
        /// </param>
        /// <returns>
        /// The JWT Bearer token parsed from the Authorization header,
        /// or nul if the uthorization header was not found
        /// or its value is not a Bearer token.
        /// </returns>
        string ParseToken(IHeaderDictionary httpRequestHeaders);
    }
}
