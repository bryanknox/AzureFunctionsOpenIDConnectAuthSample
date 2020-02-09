using System.IO;
using System.Threading.Tasks;
using AzureFunctionsOpenIDConnectAuthSample.Security;
using AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsOpenIDConnectAuthSample
{
    public class Function1
    {
        private IApiSecurity _apiSecurity;

        public Function1(IApiSecurity apiSecurity)
        {
            _apiSecurity = apiSecurity;
        }

        [FunctionName(nameof(Function1))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            AuthorizationResult authorizationResult = await _apiSecurity.Authorize(req.Headers, log);
            if (!authorizationResult.Success)
            {
                log.LogWarning(authorizationResult.FailureReason);
                return new UnauthorizedResult();
            }
            log.LogInformation("C# HTTP trigger function rquest is authorized.");

            // Parse name from query parameter.
            string name = req.Query["name"];

            // Get request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Set name to query string or body data.
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
