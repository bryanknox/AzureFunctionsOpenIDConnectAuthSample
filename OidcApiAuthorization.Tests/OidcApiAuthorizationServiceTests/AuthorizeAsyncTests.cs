using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
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
            const string AudianceForTest = "AudianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudianceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ThrowFirstTime = true
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Success);

            Assert.Equal(2, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(1, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_for_unauthorized_token()
        {
            const string AudianceForTest = "AudianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudianceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                // Normally a SecurityTokenException will be thrown when the token is not authorized.
                ExceptionToThrow = new SecurityTokenException()
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Failed);

            Assert.Equal(1, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(0, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_if_bad_Aurthorization_header()
        {
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudianceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = null // No Authorization token was found.
            };

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManager: null); // Not accessed in this test.

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders: null);

            Assert.True(result.Failed);
            
            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
                result.FailureReason);
        }

        [Fact]
        public async Task Returns_failure_if_cant_get_signing_keys_from_issuer()
        {
            const string AudianceForTest = "AudianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";
            const string ExceptionMessageForTest = "ExceptionMessageForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudianceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                ExceptionMessageForTest = ExceptionMessageForTest,
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManager);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Failed);

            Assert.StartsWith(
                "Problem getting signing keys from Open ID Connect provider (issuer).",
                result.FailureReason);
        }

        [Fact]
        public async Task Returns_success_for_happy_path()
        {
            const string AudianceForTest = "AudianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudianceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper();

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Success);
        }
    }
}
