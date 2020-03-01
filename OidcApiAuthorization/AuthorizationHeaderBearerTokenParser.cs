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
        /// The JWT Bearer token parsed from the Authorization header (without the "Bearer " prefix),
        /// or null if the Authorization header was not found, it is in an invalid format,
        /// or its value is not a Bearer token.
        /// </returns>
        public string ParseToken(IHeaderDictionary httpRequestHeaders)
        {
            // Get a StringValues object that represents the content of the Authorization header found in the given
            // headers.
            // Note that the default for a IHeaderDictionary is a StringValues object with one null string.
            var rawAuthorizationHeaderValue = httpRequestHeaders
                .SingleOrDefault(x => x.Key == "Authorization")
                .Value;

            if (rawAuthorizationHeaderValue.Count != 1)
            {
                // StringValues' Count will be zero if there is no Authorization header
                // and greater than one if there are more than one Authorization headers.
                return null;
            }

            // We got a value from the Authroization header.

            if (!AuthenticationHeaderValue.TryParse(
                    rawAuthorizationHeaderValue, // StringValues automatically convert to string.
                    out AuthenticationHeaderValue authenticationHeaderValue))
            {
                // Invalid token format.
                return null;
            }

            if (!string.Equals(
                    authenticationHeaderValue.Scheme,
                    "Bearer",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                // The Authorization header's value is not a Bearer token.
                return null;
            }

            // Return the token parsed from the Athorization header.
            // This is the token with the "Bearer " prefix removed.
            // The Parameter will be null, if nothing followed the "Bearer " prefix.
            return authenticationHeaderValue.Parameter;
        }
    }
}
