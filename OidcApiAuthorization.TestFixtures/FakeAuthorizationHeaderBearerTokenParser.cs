using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeAuthorizationHeaderBearerTokenParser : IAuthorizationHeaderBearerTokenParser
    {
        public string ParsedTokenToReturn { get; set; }

        public string ParseToken(IHeaderDictionary httpRequestHeaders)
        {
            return ParsedTokenToReturn;
        }
    }
}
