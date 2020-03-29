using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OidcApiAuthorization.Models;

namespace OidcApiAuthorization.Abstractions
{
    public interface IApiAuthorization
    {
        Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders);

        Task<HealthCheckResult> HealthCheckAsync();
    }
}
