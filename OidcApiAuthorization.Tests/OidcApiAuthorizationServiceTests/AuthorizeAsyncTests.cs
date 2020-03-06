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
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ParsedTokenForTest = "parsedTokenForTest";

            var claimsPrincipalForTest = new ClaimsPrincipal();
            var securityTokenForTest = new FakeSecurityToken();

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        AuthorizationAudience = AudianceForTest,
                        AuthorizationIssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenParser = new FakeAuthorizationHeaderBearerTokenParser()
            {
                ParsedTokenToReturn = ParsedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ThrowFirstTime = true,
                ClaimsPrincipalToReturn = claimsPrincipalForTest,
                SecurityTokenToOutput = securityTokenForTest
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = fakeOidcConfigurationManager
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManagerFactory);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Success);

            Assert.Equal(claimsPrincipalForTest, result.ClaimsPrincipal);
            Assert.Equal(securityTokenForTest, result.SecurityToken);

            Assert.Equal(2, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(1, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_for_unauthorized_token()
        {
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ParsedTokenForTest = "parsedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        AuthorizationAudience = AudianceForTest,
                        AuthorizationIssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenParser = new FakeAuthorizationHeaderBearerTokenParser()
            {
                ParsedTokenToReturn = ParsedTokenForTest
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

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = fakeOidcConfigurationManager
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManagerFactory);

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
                        AuthorizationAudience = AudianceForTest,
                        AuthorizationIssuerUrl = IssuerUrlForTest
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
                httpRequestHeaders);

            Assert.True(result.Failed);
            
            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
                result.FailureReason);
        }

        [Fact]
        public async Task Returns_failure_if_cant_get_signing_keys_from_issuer()
        {
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ParsedTokenForTest = "parsedTokenForTest";
            const string ExceptionMessageForTest = "exceptionMessageForTest";

            var listLogger = new ListLoggerFixture();

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        AuthorizationAudience = AudianceForTest,
                        AuthorizationIssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenParser = new FakeAuthorizationHeaderBearerTokenParser()
            {
                ParsedTokenToReturn = ParsedTokenForTest
            };

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = new FakeOidcConfigurationManager()
                {
                    ExceptionMessageForTest = ExceptionMessageForTest,
                }
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManagerFactory);

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
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";
            const string ParsedTokenForTest = "parsedTokenForTest";

            var claimsPrincipalForTest = new ClaimsPrincipal();
            var securityTokenForTest = new FakeSecurityToken();

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        AuthorizationAudience = AudianceForTest,
                        AuthorizationIssuerUrl = IssuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenParser = new FakeAuthorizationHeaderBearerTokenParser()
            {
                ParsedTokenToReturn = ParsedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ClaimsPrincipalToReturn = claimsPrincipalForTest,
                SecurityTokenToOutput = securityTokenForTest
            };

            var fakeOidcConfigurationManagerFactory = new FakeOidcConfigurationManagerFactory()
            {
                IOidcConfigurationManagerToReturn = new FakeOidcConfigurationManager()
                {
                    SecurityKeysForTest = new List<SecurityKey>()
                }
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManagerFactory);

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Success);

            Assert.Equal(claimsPrincipalForTest, result.ClaimsPrincipal);
            Assert.Equal(securityTokenForTest, result.SecurityToken);
        }
    }
}
