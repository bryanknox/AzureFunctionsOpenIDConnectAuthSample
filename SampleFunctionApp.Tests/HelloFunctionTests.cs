using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OidcApiAuthorization.Abstractions;
using OidcApiAuthorization.TestFixtures;
using TestFixtures.AzureFunctions;
using Xunit;

namespace SampleFunctionApp.Tests
{
    public class HelloFunctionTests
    {

        [Fact]
        public async void Authorization_fail_returns_UnauthorizedResult()
        {
            const string ExpectedFailureReason = "some reason to fail.";

            var fakeApiAuthorization = new FakeApiAuthorizationService()
            {
                // Setup for athuorization fails.
                ApiAuthorizationResultForTests = new ApiAuthorizationResult(ExpectedFailureReason)
            };

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpGetRequest(
                "name", // quertyStringKey doesn't matter.
                "world"); // quertyStringValue doesn't matter.

            var listLogger = new ListLoggerFixture();

            var func = new HelloFunction(fakeApiAuthorization);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<UnauthorizedResult>(actionResult);

            Assert.NotEmpty(listLogger.LogEntries);

            Assert.True(listLogger.HasLogEntryMessageContaining(
                LogLevel.Warning,
                ExpectedFailureReason));
        }

        [Fact]
        public async void Happy_path_returns_OkObjectResult_with_hello_text()
        {
            const string ExpecetedName = "Some Name";

            var fakeApiAuthorization = new FakeApiAuthorizationService()
            {
                // Setup to fake athuorization success.
                ApiAuthorizationResultForTests = new ApiAuthorizationResult()
            };

            string jsonBody = $"{{ \"name\": \"{ExpecetedName}\" }}";

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpPostRequest(
                jsonBody);

            var listLogger = new ListLoggerFixture();

            var func = new HelloFunction(fakeApiAuthorization);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<OkObjectResult>(actionResult);

            Assert.Equal($"Hello, {ExpecetedName}", ((OkObjectResult)actionResult).Value);

            Assert.NotEmpty(listLogger.LogEntries);

            Assert.True(listLogger.HasLogEntryMessageContaining(
                LogLevel.Warning,
                "C# HTTP trigger function rquest is authorized."));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Just a space character.
        [InlineData("{}")]
        [InlineData("{ \"name\": \"\" }")] // Empty string name value.
        [InlineData("{ \"name\": \" \" }")] // Just a space for name value.
        public async void No_name_returns_BadRequestObjectResult_with_help_text(string jsonBody)
        {
            var fakeApiAuthorizationService = new FakeApiAuthorizationService()
            {
                // Setup to fake athuorization success.
                ApiAuthorizationResultForTests = new ApiAuthorizationResult()
            };

            HttpRequest httpRequest = HttpRequestFactoryFixture.CreateHttpPostRequest(
                jsonBody);

            var listLogger = new ListLoggerFixture();

            var func = new HelloFunction(fakeApiAuthorizationService);

            IActionResult actionResult = await func.Run(httpRequest, listLogger);

            Assert.NotNull(actionResult);

            Assert.IsType<BadRequestObjectResult>(actionResult);

            Assert.Equal("Please pass a name the request body.", ((BadRequestObjectResult)actionResult).Value);

            Assert.NotEmpty(listLogger.LogEntries);
        }

    }
}
