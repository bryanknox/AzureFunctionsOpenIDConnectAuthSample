using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OidcApiAuthorization.Abstractions
{
    // TODO: Rename to IApiauthorizer
    public interface IApiAuthorization
    {
        // TODO: Rename to AuthorizeAsync(..)
        Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log);
    }
}
