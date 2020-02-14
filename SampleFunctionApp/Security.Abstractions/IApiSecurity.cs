using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsOpenIDConnectAuthSample.Security.Abstractions
{
    public interface IApiSecurity
    {
        Task<AuthorizationResult> Authorize(IHeaderDictionary httpRequestHeaders, ILogger log);
    }
}
