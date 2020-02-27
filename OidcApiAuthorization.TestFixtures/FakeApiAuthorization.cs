using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization.TestFixtures
{
    public class FakeApiAuthorization : IApiAuthorization
    {
        public AuthorizationResult AuthorizationResultForTests { get; set; }

        // IApiAuthorization members.

        public async Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log)
        {
            // Prevent compiler Warning CS1998 "This async method lacks 'await' operators and ..."
            await Task.FromResult(0);

            return AuthorizationResultForTests;
        }
    }
}
