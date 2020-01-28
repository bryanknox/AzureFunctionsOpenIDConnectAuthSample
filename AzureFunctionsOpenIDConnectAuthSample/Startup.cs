using AzureFunctionsOpenIDConnectAuthSample.Security;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionsOpenIDConnectAuthSample.Startup))]
namespace AzureFunctionsOpenIDConnectAuthSample
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddApiSecurity();
        }
    }
}
