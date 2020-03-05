using System;
using System.Threading.Tasks;
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
        public async Task Retrys_once_if_SecurityTokenSignatureKeyNotFoundException()
        {
            // TODO: Implement Retrys_once_if_SecurityTokenSignatureKeyNotFoundException()
            Assert.True(false);
        }

        [Fact]
        public async Task Returns_failure_for_unauthorized_token()
        {
            // TODO: Implement Returns_failure_for_unauthorized_token()
            Assert.True(false);
        }

        [Fact]
        public async Task Returns_failure_if_bad_Aurthorization_header()
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

        [Fact]
        public async Task Returns_success_for_happy_path()
        {
            // TODO: Implement Returns_success_for_happy_path()
            Assert.True(false);
        }

        [Fact]
        public async Task Throws_if_cant_get_signing_keys_from_issuer()
        {
            const string audianceForTest = "audianceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string parsedTokenForTest = "parsedTokenForTest";
            const string exceptionMessageForTest = "exceptionMessageForTest";

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
                ParsedTokenToReturn = parsedTokenForTest
            };

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = new FakeOidcConfigurationManager()
                {
                    ExceptionMessageForTest = exceptionMessageForTest,
                    GetIssuerSigningKeysAsyncShouldThrow = true
                }
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManagerFactory);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.AuthorizeAsync(
                    httpRequestHeaders,
                    listLogger)
            );

            Assert.StartsWith("Problem getting signing keys from Open ID Connect provider", ex.Message);

            Assert.IsType<TestException>(ex.InnerException);
            Assert.Equal(exceptionMessageForTest, ex.InnerException.Message);
        }

    }
}
