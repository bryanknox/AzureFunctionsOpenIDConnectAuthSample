using HttpFunctionAppAuth0Play.Security;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(HttpFunctionAppAuth0Play.Startup))]
namespace HttpFunctionAppAuth0Play
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddApiSecurity();
        }
    }
}
