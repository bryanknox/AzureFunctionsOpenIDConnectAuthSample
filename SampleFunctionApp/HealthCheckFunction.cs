using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OidcApiAuthorization.Abstractions;
using OidcApiAuthorization.Models;

namespace SampleFunctionApp
{
    public class HealthCheckFunction
    {
        private readonly IApiAuthorization _apiAuthorization;

        public HealthCheckFunction(IApiAuthorization apiAuthorization)
        {
            _apiAuthorization = apiAuthorization;
        }

        [FunctionName("HealthCheckFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogWarning($"HTTP trigger function {nameof(HealthCheckFunction)} received a request.");

            HealthCheckResult result = await _apiAuthorization.HealthCheckAsync();

            if (result.IsHealthy)
            {
                log.LogWarning($"{nameof(HealthCheckFunction)} health check OK.");
            }
            else
            {
                log.LogError(
                    $"{nameof(HealthCheckFunction)} health check failed."
                      + $" {nameof(HealthCheckResult)}: {JsonConvert.SerializeObject(result)}"
                    );
            }
            return new OkObjectResult(result);
        }
    }
}
