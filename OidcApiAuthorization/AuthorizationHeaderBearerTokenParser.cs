using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization
{
    public class AuthorizationHeaderBearerTokenParser : IAuthorizationHeaderBearerTokenParser
    {
        /// <summary>
        /// Parses the Bearer token from the Authorization header of the given HTTP request headers.
        /// </summary>
        /// <param name="headers">
        /// The headers from an HTTP request.
        /// </param>
        /// <returns>
        /// The JWT Bearer token parsed from the Authorization header,
        /// or null if the uthorization header was not found
        /// or its value is not a Bearer token.
        /// </returns>
        public string ParseToken(IHeaderDictionary httpRequestHeaders)
        {
            // Get a StringValues object that represents the content of the Authorization header found in the given
            // headers.
            var rawAuthorizationHeaderValue = httpRequestHeaders.SingleOrDefault(x => x.Key == "Authorization").Value;
            if (rawAuthorizationHeaderValue.Count != 1)
            {
                // StringValues' Count will be zero if there is no Authorization header
                // and greater than one if there are more than one Authorization headers.
                return null;
            }
            string rawAuthorizationHeaderValueString = rawAuthorizationHeaderValue;

            AuthenticationHeaderValue authenticationHeaderValue = AuthenticationHeaderValue.Parse(rawAuthorizationHeaderValueString);

            if (authenticationHeaderValue == null
                || !string.Equals(authenticationHeaderValue.Scheme, "Bearer", StringComparison.InvariantCultureIgnoreCase))
            {
                // The Authorization header was not found, or its value was not a Bearer token.
                return null;
            }

            // Return the token parsed from the Athorization header.
            return authenticationHeaderValue.Parameter;
        }
    }
}
