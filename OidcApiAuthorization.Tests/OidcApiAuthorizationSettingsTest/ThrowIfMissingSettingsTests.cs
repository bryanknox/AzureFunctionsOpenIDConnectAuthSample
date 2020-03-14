using System;
using OidcApiAuthorization;
using Xunit;

namespace OidcApiAuthorizationSettingsTest
{
    public class ThrowIfMissingSettingsTests
    {
        [Fact]
        public void Dosent_throw_if_none_missing()
        {
            var settings = new OidcApiAuthorizationSettings()
            {
                AuthorizationAudience = "someAudience",
                AuthorizationIssuerUrl = "https://my.test.url/"
            };

            settings.ThrowIfMissingSettings();

            Assert.True(true);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Spaces.
        public void Throws_if_AuthorizationAudience_missing(string audience)
        { 
            var settings = new OidcApiAuthorizationSettings()
            {
                AuthorizationAudience = audience,
                AuthorizationIssuerUrl = "https://my.test.url/"
            };

            var ex = Assert.Throws<Exception>(() => settings.ThrowIfMissingSettings());

            Assert.Equal(
                $"Missing application setting. {nameof(OidcApiAuthorizationSettings.AuthorizationAudience)} setting is not set.",
                ex.Message);
        }

        [Fact]
        public void Throws_if_AuthorizationIssuerUrl_missing()
        {
            string[] missingValues = new string[] { null, string.Empty, "  " };
            foreach (string missingValue in missingValues)
            {
                var settings = new OidcApiAuthorizationSettings()
                {
                    AuthorizationAudience = "someAudience",
                    AuthorizationIssuerUrl = missingValue
                };

                var ex = Assert.Throws<Exception>(() => settings.ThrowIfMissingSettings());

                Assert.Equal(
                    $"Missing application setting. {nameof(OidcApiAuthorizationSettings.AuthorizationIssuerUrl)} setting is not set.",
                    ex.Message);
            }
        }

    }
}
