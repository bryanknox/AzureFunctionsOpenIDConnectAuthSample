using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OidcApiAuthorization.Abstractions
{
    public interface IApiAuthorization
    {
        Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders);
    }
}
