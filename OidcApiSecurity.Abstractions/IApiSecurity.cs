using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OidcApiSecurity.Abstractions
{
    public interface IApiSecurity
    {
        Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log);
    }
}
