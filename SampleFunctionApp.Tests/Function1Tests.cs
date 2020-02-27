using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OidcApiAuthorization.Abstractions;
using OidcApiAuthorization.TestFixtures;
using TestFixtures.AzureFunctions;
using Xunit;

namespace SampleFunctionApp.Tests
{
    public class Function1Tests
    {
        [Fact]
        public async void Returns_UnauthorizedResult_if_authorization_fails()
        {
            const string expectedFailureReason = "some reason to fail.";

            var fakeApiAuthorization = new FakeApiAuthorization()
            {
                // Setup for athuorization fails.
                AuthorizationResultForTests = new AuthorizationResult(expectedFailureReason)
            };

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpGetRequest(
                "name", // quertyStringKey doesn't matter.
                "world"); // quertyStringValue doesn't matter.

            var listLogger = new ListLoggerFixture();

            var func = new Function1(fakeApiAuthorization);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<UnauthorizedResult>(actionResult);

            Assert.NotEmpty(listLogger.LogEntries);

            Assert.True(listLogger.HasLogEntryMessageContaining(
                LogLevel.Warning,
                expectedFailureReason));
        }
    }
}
