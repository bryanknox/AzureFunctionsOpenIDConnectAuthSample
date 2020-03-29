using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Abstractions;
using OidcApiAuthorization.Models;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeApiAuthorizationService : IApiAuthorization
    {
        public ApiAuthorizationResult ApiAuthorizationResultForTests { get; set; }

        public string BadHealthMessageForTests { get; set; }

        // IApiAuthorization members.

        public async Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders)
        {
            return await Task.FromResult(ApiAuthorizationResultForTests);
        }

        public async Task<HealthCheckResult> HealthCheckAsync()
        {
            return await Task.FromResult(
                new HealthCheckResult(BadHealthMessageForTests));
        }
    }
}
