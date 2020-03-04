using System;
using OidcApiAuthorization;
using OidcApiAuthorization.TestFixtures;
using TestFixtures.AzureFunctions;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_throws_if_missing_settings()
        {
            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        // No setting are set for this test.
                        AuthorizationAudience = null,
                        AuthorizationIssuerUrl = null
                    }
                };

            var ex = Assert.Throws<Exception>(() =>
                new OidcApiAuthorizationService(
                    fakeApiAuthorizationSettingsOptions,
                    authorizationHeaderBearerTokenParser: null, // Not accessed in this test.
                    jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                    oidcConfigurationManagerFactory: null) // Not accessed in this test.
            );

            Assert.StartsWith("Missing application setting.",  ex.Message);
        }

        [Fact]
        public void Constructor_uses_configuration_manager_factory()
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

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenParser: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManagerFactory);

            // Test that the issuer URL was sent to the Oidc Configuration Manager Factory.
            Assert.Equal(issuerUrlForTest, fakeOidcConfigurationManagerFactory.IssuerUrlReceived);
        }


    }
}
