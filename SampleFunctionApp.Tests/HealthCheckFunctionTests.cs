using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OidcApiAuthorization.Models;
using OidcApiAuthorization.TestFixtures;
using TestFixtures.AzureFunctions;
using Xunit;

namespace SampleFunctionApp.Tests
{
    public class HealthCheckFunctionTests
    {
        [Fact]
        public async void Health_check_is_healthy()
        {
            var fakeApiAuthorization = new FakeApiAuthorizationService();

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpGetRequest();

            var listLogger = new ListLoggerFixture();

            var func = new HealthCheckFunction(fakeApiAuthorization);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<OkObjectResult>(actionResult);

            Assert.IsType<HealthCheckResult>(((OkObjectResult)actionResult).Value);

            var healthCheckResult = (HealthCheckResult)((OkObjectResult)actionResult).Value;

            Assert.True(healthCheckResult.IsHealthy);
            Assert.Null(healthCheckResult.BadHealthMessage);
        }

        [Fact]
        public async void Health_check_is_not_healthy()
        {
            const string ExpectedBadHealthMessage = "Bad health mess.";

            var fakeApiAuthorization = new FakeApiAuthorizationService()
            {
                BadHealthMessageForTests = ExpectedBadHealthMessage
            };

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpGetRequest();

            var listLogger = new ListLoggerFixture();

            var func = new HealthCheckFunction(fakeApiAuthorization);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<OkObjectResult>(actionResult);

            Assert.IsType<HealthCheckResult>(((OkObjectResult)actionResult).Value);

            var healthCheckResult = (HealthCheckResult)((OkObjectResult)actionResult).Value;

            Assert.False(healthCheckResult.IsHealthy);
            Assert.Equal(ExpectedBadHealthMessage, healthCheckResult.BadHealthMessage);

            Assert.NotEmpty(listLogger.LogEntries);

            Assert.True(listLogger.HasLogEntryMessageThatStartsWith(
                LogLevel.Error,
                $"{nameof(HealthCheckFunction)} health check failed."));           
        }

    }
}
