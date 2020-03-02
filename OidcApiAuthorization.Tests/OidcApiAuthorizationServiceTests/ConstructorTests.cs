using System;
using Microsoft.Extensions.Options;
using Xunit;

namespace OidcApiAuthorization.Tests.OidcApiAuthorizationServiceTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_throws_if_missing_settings()
        {
            const string expectedExceptionMessage = "some message";

            IOptions<OidcApiAuthorizationSettings> fakeApiAuthorizationSettingsOptions = null;
            // TODO: FakeApiAuthorizationSettingsOptions class.

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenParser: null,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManagerFactory: null); // Not accessed in this test.

            var ex = Assert.Throws<Exception>(() =>
                new OidcApiAuthorizationService(
                    fakeApiAuthorizationSettingsOptions,
                    authorizationHeaderBearerTokenParser: null, // Not accessed in this test.
                    jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                    oidcConfigurationManagerFactory: null) // Not accessed in this test.
            );

            Assert.Equal(expectedExceptionMessage,  ex.Message);
        }

    }
}
