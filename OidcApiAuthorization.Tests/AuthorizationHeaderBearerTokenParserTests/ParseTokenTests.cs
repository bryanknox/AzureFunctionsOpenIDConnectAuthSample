using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using OidcApiAuthorization;
using Xunit;

namespace AuthorizationHeaderBearerTokenParserTests
{
    public class ParseTokenTests
    {
        [Fact]
        public void Returns_Bearer_token()
        {
            const string expectedToken = "some-token-value";

            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", $"Bearer {expectedToken}"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var parser = new AuthorizationHeaderBearerTokenParser();

            string token = parser.ParseToken(httpRequestHeaders);

            Assert.NotNull(token);

            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public void Doesnt_care_about_bEaRer_case()
        {
            const string expectedToken = "some-token-value";

            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", $"bEaRer {expectedToken}"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var parser = new AuthorizationHeaderBearerTokenParser();

            string token = parser.ParseToken(httpRequestHeaders);

            Assert.NotNull(token);

            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public void Returns_null_if_no_Authorization_header()
        {
            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("header2", "header2value"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var parser = new AuthorizationHeaderBearerTokenParser();

            string token = parser.ParseToken(httpRequestHeaders);

            Assert.Null(token);
        }

        [Fact]
        public void Returns_null_if_multiple_Athorization_headers()
        {
            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),

                // Multiple Authroization headers are grouped under the same key.
                new KeyValuePair<string, StringValues>(
                    "Authorization", 
                    new StringValues(new string[] { "header2value", "anotherValue" })),

                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var parser = new AuthorizationHeaderBearerTokenParser();

            string token = parser.ParseToken(httpRequestHeaders);

            Assert.Null(token);
        }

        [Fact]
        public void Returns_null_if_not_Bearer_token()
        {
            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", "header2value"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var parser = new AuthorizationHeaderBearerTokenParser();

            string token = parser.ParseToken(httpRequestHeaders);

            Assert.Null(token);
        }

    }
}
