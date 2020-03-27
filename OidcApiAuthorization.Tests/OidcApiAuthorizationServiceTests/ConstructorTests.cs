using System;
using OidcApiAuthorization;
using OidcApiAuthorization.TestFixtures;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Throws_if_missing_settings()
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
                    authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                    jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                    oidcConfigurationManager: null) // Not accessed in this test.
            );

            // Check that the exception thrown is the one we're lookging for.
            // We are NOT trying to enforce a requirement on the particular
            // message text.
            Assert.StartsWith("Missing application setting.",  ex.Message);
        }
    }
}
