using OidcApiAuthorization;
using OidcApiAuthorization.Models;
using OidcApiAuthorization.TestFixtures;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class HealthCheckAsyncTests
    {
        private readonly string _expectedMissingSettingsMessage =
            $"Some or all {nameof(OidcApiAuthorizationSettings)} are missing.";

        [Fact]
        public async void Returns_error_if_missing_options()
        {
            var service = new OidcApiAuthorizationService(
                null, // Options are missing.
                authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManager: null); ; // Not accessed in this test.

            HealthCheckResult result = await service.HealthCheckAsync();

            Assert.False(result.IsHealthy);
            Assert.Equal(_expectedMissingSettingsMessage, result.BadHealthMessage);
        }

        [Fact]
        public async void Returns_error_if_missing_all_settings()
        {
            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = null // No settings.
                };

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManager: null); ; // Not accessed in this test.

            HealthCheckResult result = await service.HealthCheckAsync();

            Assert.False(result.IsHealthy);
            Assert.Equal(_expectedMissingSettingsMessage, result.BadHealthMessage);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")] // Spaces.
        [InlineData("someAudience", null)]
        [InlineData(null, "https://issuerUrl.for.test/")]
        public async void Returns_error_if_missing_settings_values(string Audience, string issuerUrl)
        {
            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = Audience,
                        IssuerUrl = issuerUrl
                    }
                };

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManager: null); ; // Not accessed in this test.

            HealthCheckResult result = await service.HealthCheckAsync();

            Assert.False(result.IsHealthy);
            Assert.Equal(_expectedMissingSettingsMessage, result.BadHealthMessage);
        }

        [Fact]
        public async void Returns_error_if_throws_getting_signingkeys()
        {
            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = "AudienceForTest",
                        IssuerUrl = "https://issuerUrl.for.test/"
                    }
                };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                ExceptionMessageForTest = "Some exception mess."
            };

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManager);

            HealthCheckResult result = await service.HealthCheckAsync();

            Assert.False(result.IsHealthy);
            Assert.StartsWith(
                "Problem getting signing keys from Open ID Connect provider (issuer).",
                result.BadHealthMessage);
        }

        [Fact]
        public async void Returns_no_error_if_healthy()
        {
            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = "AudienceForTest",
                        IssuerUrl = "https://issuerUrl.for.test/"
                    }
                };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager();

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                authorizationHeaderBearerTokenExractor: null, // Not accessed in this test.
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManager);

            HealthCheckResult result = await service.HealthCheckAsync();

            Assert.True(result.IsHealthy);
            Assert.Null(result.BadHealthMessage);
        }
    }
}
