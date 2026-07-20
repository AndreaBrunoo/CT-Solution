using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Frontend.Infrastructure.Api;

namespace Frontend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<AuthHeaderHandler>();

        var apiBaseUrl = config["ApiBaseUrl"];

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new Exception("ApiBaseUrl non configurato in appsettings.json");

        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }
}