using OidcApiAuthorization;
using Xunit;

namespace OidcApiAuthorizationSettingsTest
{
    public class AuthorizationIssuerUrlTests
    {
        [Fact]
        public void Appends_missing_foward_slash()
        {
            const string withoutEndingSlash = "https://my.test.url";
            const string withEndingSlash = "https://my.test.url/";

            var settings = new OidcApiAuthorizationSettings()
            {
                AuthorizationIssuerUrl = withoutEndingSlash
            };

            Assert.Equal(withEndingSlash, settings.AuthorizationIssuerUrl);
        }

        [Fact]
        public void Defaults_to_null()
        {
            var settings = new OidcApiAuthorizationSettings();

            Assert.Null(settings.AuthorizationIssuerUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Spaces.
        [InlineData("https://my.test.url/")]
        public void Doesnt_append_foward_slash(string issuerUrl)
        {
            var settings = new OidcApiAuthorizationSettings()
            {
                AuthorizationIssuerUrl = issuerUrl
            };

            Assert.Equal(issuerUrl, settings.AuthorizationIssuerUrl);
        }
    }
}
