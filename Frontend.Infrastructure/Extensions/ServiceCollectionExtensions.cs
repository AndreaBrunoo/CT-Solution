using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Frontend.Infrastructure.Api;

namespace Frontend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<AuthHeaderHandler>();

        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(config["Api:BaseUrl"]);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }
}