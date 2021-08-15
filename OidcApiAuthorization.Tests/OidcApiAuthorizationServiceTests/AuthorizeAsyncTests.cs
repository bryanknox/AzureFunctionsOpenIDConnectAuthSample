using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OidcApiAuthorization;
using OidcApiAuthorization.Models;
using OidcApiAuthorization.TestFixtures;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class AuthorizeAsyncTests
    {
        [Fact]
        public async Task Retrys_once_if_SecurityTokenSignatureKeyNotFoundException()
        {
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
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
        public async Task Returns_failure_if_SecurityTokenSignatureKeyNotFoundException_on_retry()
        {
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ThrowFirstTime = true, ThrowSecondTime = true
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

            Assert.False(result.Success);

            Assert.Equal(2, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(1, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [InlineData("SecurityTokenException")] // Normally throws SecurityTokenException when token is not authorized.
        [InlineData("Exception")] // Test when throws any other exception type too.
        [Theory]
        public async Task Returns_failure_for_unauthorized_token(string exceptionTypeToThrow)
        {
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            Exception exceptionToThrow = exceptionTypeToThrow == "SecurityTokenException"
                    ? new SecurityTokenException()
                    : new Exception();

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
                        IssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = ExtractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                // Throw for unauthrorized token. 
                ExceptionToThrow = exceptionToThrow
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
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
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
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";
            const string ExceptionMessageForTest = "ExceptionMessageForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
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
            const string AudienceForTest = "AudienceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ExtractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = AudienceForTest,
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
