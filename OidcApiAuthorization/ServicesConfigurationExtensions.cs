using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OidcApiAuthorization.Abstractions;

namespace OidcApiAuthorization
{
    public static class ServicesConfigurationExtensions
    {
        public static void AddOidcApiAuthorization(this IServiceCollection services)
        {
            // This also needs to be a singleton, because the singledton IApiAuthorization requires it.
            services.AddSingleton<IAuthorizationHeaderBearerTokenParser, AuthorizationHeaderBearerTokenParser>();

            // This also needs to be a singleton, because the singledton IApiAuthorization requires it.
            services.AddSingleton<IJwtSecurityTokenHandlerWrapper, JwtSecurityTokenHandlerWrapper>();

            // This also needs to be a singleton, because the singledton IApiAuthorization requires it.
            services.AddSingleton<IOidcConfigurationManagerFactory, OidcConfigurationManagerFactory>();

            // Setup injection of OidcApiAuthorizationSettings configured in the
            // Function's app settings (or local.settings.json)
            // as IOptions<OidcApiAuthorizationSettings>.
            // See https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings
            services.AddOptions<OidcApiAuthorizationSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(OidcApiAuthorizationSettings)).Bind(settings);
                });

            // IApiAuthorization is created as a singleton, so that only one instance
            // is created for the lifetime of the hosting Azure Function App.
            // That helps reduce the number of calls to the authorization service
            // for the signing keys and other stuff that can be used across multiple
            // calls to the HTTP triggered Azure Functions.
            services.AddSingleton<IApiAuthorization, OidcApiAuthorizationService>();
        }
    }
}
