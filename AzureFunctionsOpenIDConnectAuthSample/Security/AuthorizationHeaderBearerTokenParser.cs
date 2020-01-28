using System;
using System.Linq;
using System.Net.Http.Headers;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AzureFunctionsOpenIDConnectAuthSample.Security
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
        /// or nul if the uthorization header was not found
        /// or its value is not a Bearer token.
        /// </returns>
        public string ParseToken(IHeaderDictionary httpRequestHeaders)
        {
            // Get a StringValues object that represents the content of the Authorization header found in the given
            // headers. If the Authorization header is not found the StringValues.Value returned will be null.
            // Default for a KeyValuePair<string, StringValues> has a Value that is a StringValue with a null string.
            string rawAuthorizationHeaderValueString = httpRequestHeaders.SingleOrDefault(x => x.Key == "Authorization").Value;

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
