using System;
using OidcApiAuthorization;
using OidcApiAuthorization.TestFixtures;
using Xunit;

namespace OidcApiAuthorizationHealthCheckTests
{
    public class ThrowIfMissingSettingsTests
    {
        [Fact]
        public void Dosent_throw_if_none_missing()
        {
            var settings = new OidcApiAuthorizationSettings()
            {
                Audience = "someAudience",
                IssuerUrl = "https://my.test.url/"
            };

            var fakeOptions = new FakeOptions<OidcApiAuthorizationSettings>()
            {
                Value = settings
            };

            var healthCheck = new OidcApiAuthorizationHealthCheck(fakeOptions);

            healthCheck.ThrowIfMissingSettings();

            Assert.True(true);
        }

        [Fact]
        public void Throws_if_all_settings_missing()
        {
            var fakeOptions = new FakeOptions<OidcApiAuthorizationSettings>()
            {
                Value = null
            };

            var healthCheck = new OidcApiAuthorizationHealthCheck(fakeOptions);

            var ex = Assert.Throws<Exception>(() => healthCheck.ThrowIfMissingSettings());

            Assert.Equal(
                $"Some or all {nameof(OidcApiAuthorizationSettings)} are missing.",
                ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Spaces.
        public void Throws_if_Audience_missing(string audience)
        { 
            var settings = new OidcApiAuthorizationSettings()
            {
                Audience = audience,
                IssuerUrl = "https://my.test.url/"
            };

            var fakeOptions = new FakeOptions<OidcApiAuthorizationSettings>()
            {
                Value = settings
            };

            var healthCheck = new OidcApiAuthorizationHealthCheck(fakeOptions);

            var ex = Assert.Throws<Exception>(() => healthCheck.ThrowIfMissingSettings());

            Assert.Equal(
                $"Some or all {nameof(OidcApiAuthorizationSettings)} are missing.",
                ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Spaces.
        public void Throws_if_IssuerUrl_missing(string issuerUrl)
        {
            var settings = new OidcApiAuthorizationSettings()
            {
                Audience = "someAudience",
                IssuerUrl = issuerUrl
            };

            var fakeOptions = new FakeOptions<OidcApiAuthorizationSettings>()
            {
                Value = settings
            };

            var healthCheck = new OidcApiAuthorizationHealthCheck(fakeOptions);

            var ex = Assert.Throws<Exception>(() => healthCheck.ThrowIfMissingSettings());

            Assert.Equal(
                $"Some or all {nameof(OidcApiAuthorizationSettings)} are missing.",
                ex.Message);
        }
    }
}
