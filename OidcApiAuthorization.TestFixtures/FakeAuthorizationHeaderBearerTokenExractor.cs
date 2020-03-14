using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeAuthorizationHeaderBearerTokenExractor : IAuthorizationHeaderBearerTokenExtractor
    {
        public string TokenToReturn { get; set; }

        public string GetToken(IHeaderDictionary httpRequestHeaders)
        {
            return TokenToReturn;
        }
    }
}
