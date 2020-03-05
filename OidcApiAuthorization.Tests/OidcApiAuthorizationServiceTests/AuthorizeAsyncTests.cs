using System;
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
            // TODO: Implement Retrys_once_if_SecurityTokenSignatureKeyNotFoundException()
            Assert.True(false);

            // Prevent compiler Warning CS1998 "This async method lacks 'await' operators and ..."
            await Task.FromResult(0);
        }

        [Fact]
        public async Task Returns_failure_for_unauthorized_token()
        {
            // TODO: Implement Returns_failure_for_unauthorized_token()
            Assert.True(false);
            
            // Prevent compiler Warning CS1998 "This async method lacks 'await' operators and ..."
            await Task.FromResult(0);
        }

        [Fact]
        public async Task Returns_failure_if_bad_Aurthorization_header()
        {
            const string AudianceForTest = "audianceForTest";
            const string IssuerUrlForTest = "https://issuerUrl.for.test/";

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

            Assert.True(result.Failed);
            
            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
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
                httpRequestHeaders,
                listLogger);

            Assert.True(result.Success);

            Assert.Equal(claimsPrincipalForTest, result.ClaimsPrincipal);
            Assert.Equal(securityTokenForTest, result.SecurityToken);
        }

        [Fact]
        public async Task Throws_if_cant_get_signing_keys_from_issuer()
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
            Assert.Equal(ExceptionMessageForTest, ex.InnerException.Message);
        }

    }
}
