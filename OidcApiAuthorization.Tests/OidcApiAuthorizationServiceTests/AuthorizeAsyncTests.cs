using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OidcApiAuthorization;
using OidcApiAuthorization.Abstractions;
using TestFixtures.AzureFunctions;
using Xunit;

namespace OidcApiAuthorizationServiceTests
{
    public class AuthorizeAsyncTests
    {
        [Fact]
        public async void Returns_failure_if_bad_Aurthorization_header()
        {
            var listLogger = new ListLoggerFixture();

            IOptions<OidcApiAuthorizationSettings> fakeApiAuthorizationSettingsOptions = null;

            IAuthorizationHeaderBearerTokenParser fakeAuthorizationHeaderBearerTokenParser = null;

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenParser,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManagerFactory: null); // Not accessed in this test.

            ApiAuthorizationResult result = await service.AuthorizeAsync(
                httpRequestHeaders,
                listLogger);

            Assert.False(result.Success);
            
            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
                result.FailureReason);
        }
    }
}
