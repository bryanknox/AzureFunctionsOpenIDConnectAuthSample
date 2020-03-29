using OidcApiAuthorization;
using Xunit;

namespace OidcApiAuthorizationSettingsTest
{
    public class IssuerUrlTests
    {
        [Fact]
        public void Appends_missing_foward_slash()
        {
            const string WithoutEndingSlash = "https://my.test.url";
            const string WithEndingSlash = "https://my.test.url/";

            var settings = new OidcApiAuthorizationSettings()
            {
                IssuerUrl = WithoutEndingSlash
            };

            Assert.Equal(WithEndingSlash, settings.IssuerUrl);
        }

        [Fact]
        public void Defaults_to_null()
        {
            var settings = new OidcApiAuthorizationSettings();

            Assert.Null(settings.IssuerUrl);
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
                IssuerUrl = issuerUrl
            };

            Assert.Equal(issuerUrl, settings.IssuerUrl);
        }
    }
}
