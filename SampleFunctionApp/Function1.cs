using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OidcApiAuthorization.Abstractions;

namespace SampleFunctionApp
{
    public class Function1
    {
        private IApiAuthorization _apiAuthorization;

        public Function1(IApiAuthorization apiAuthorization)
        {
            _apiAuthorization = apiAuthorization;
        }

        [FunctionName(nameof(Function1))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogWarning("C# HTTP trigger function received a request.");

            AuthorizationResult authorizationResult = await _apiAuthorization.Authorize(req.Headers, log);
            if (!authorizationResult.Success)
            {
                log.LogWarning(authorizationResult.FailureReason);
                return new UnauthorizedResult();
            }
            log.LogWarning("C# HTTP trigger function rquest is authorized.");

            // Get name from request body.
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;

            return !string.IsNullOrWhiteSpace(name)
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name the request body.");
        }
    }
}
