using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using OidcApiAuthorization;

[assembly: FunctionsStartup(typeof(SampleFunctionApp.Startup))]
namespace SampleFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOidcApiAuthorization();
        }
    }
}
