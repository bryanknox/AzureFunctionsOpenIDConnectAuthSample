using Microsoft.AspNetCore.Http;

namespace OidcApiAuthorization.Abstractions
{
    public interface IAuthorizationHeaderBearerTokenExtractor
    {
        /// <summary>
        /// Extract the Bearer token from the Authorization header of the given HTTP request headers.
        /// </summary>
        /// <param name="headers">
        /// The headers from an HTTP request.
        /// </param>
        /// <returns>
        /// The Bearer token extracted from the Authorization header,
        /// or null if the authorization header was not found
        /// or its value is not a Bearer token.
        /// </returns>
        string GetToken(IHeaderDictionary httpRequestHeaders);
    }
}
