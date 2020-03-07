using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeApiAuthorizationService : IApiAuthorization
    {
        public ApiAuthorizationResult ApiAuthorizationResultForTests { get; set; }

        // IApiAuthorization members.

        public async Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders)
        {
            // Prevent compiler Warning CS1998 "This async method lacks 'await' operators and ..."
            await Task.FromResult(0);

            return ApiAuthorizationResultForTests;
        }
    }
}
