using Microsoft.AspNetCore.Http;
using OidcApiAuthorization;
using OidcApiAuthorization.Abstractions;
using OidcApiAuthorization.TestFixtures;
using TestFixtures.AzureFunctions;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class AuthorizeAsyncTests
    {
        [Fact]
        public async void Returns_failure_if_bad_Aurthorization_header()
        {
            const string audianceForTest = "audianceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";

            var listLogger = new ListLoggerFixture();

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        AuthorizationAudience = audianceForTest,
                        AuthorizationIssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenParser = new FakeAuthorizationHeaderBearerTokenParser()
            {
                ParsedTokenToReturn = null // No Authorization token was found.
            };

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = null // Not  accessed in this test.
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManagerFactory);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders,
                listLogger);

            Assert.False(result.Success);
            
            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
                result.FailureReason);
        }
    }
}
