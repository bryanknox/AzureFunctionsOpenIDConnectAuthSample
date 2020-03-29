using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using OidcApiAuthorization;
using Xunit;

namespace AuthorizationHeaderBearerTokenExtractorTests
{
    public class GetTokenTests
    {

        [Fact]
        public void Doesnt_care_about_bEaRer_case()
        {
            const string ExpectedToken = "some-token-value";

            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", $"bEaRer {ExpectedToken}"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var extractor = new AuthorizationHeaderBearerTokenExtractor();

            string token = extractor.GetToken(httpRequestHeaders);

            Assert.NotNull(token);

            Assert.Equal(ExpectedToken, token);
        }

        [Theory]
        [InlineData("some-token-value")]
        public void Returns_Bearer_token(string tokenValue)
        {
            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", $"Bearer {tokenValue}"),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var extractor = new AuthorizationHeaderBearerTokenExtractor();

            string token = extractor.GetToken(httpRequestHeaders);

            Assert.NotNull(token);

            Assert.Equal(tokenValue, token);
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

            var extractor = new AuthorizationHeaderBearerTokenExtractor();

            string token = extractor.GetToken(httpRequestHeaders);

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

            var extractor = new AuthorizationHeaderBearerTokenExtractor();

            string token = extractor.GetToken(httpRequestHeaders);

            Assert.Null(token);
        }

        [Theory]
        [InlineData("Bearer tokenCanNot,HaveAComma")]
        [InlineData("Bearer")]
        [InlineData("Bearer ")]
        [InlineData("tokenCanNot,HaveAComma")]
        [InlineData("NotABearerToken")]
        public void Returns_null_if_not_or_bad_Bearer_token(string invalidToken)
        {
            var httpRequestHeaders = new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("header1", "header1value"),
                new KeyValuePair<string, StringValues>("Authorization", invalidToken),
                new KeyValuePair<string, StringValues>("header3", "header3value")
            };

            var extractor = new AuthorizationHeaderBearerTokenExtractor();

            string token = extractor.GetToken(httpRequestHeaders);

            Assert.Null(token);
        }

    }
}
