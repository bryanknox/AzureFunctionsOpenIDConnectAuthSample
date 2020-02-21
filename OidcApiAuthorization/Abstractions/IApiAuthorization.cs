using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OidcApiAuthorization.Abstractions
{
    public interface IApiAuthorization
    {
        Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log);
    }
}
