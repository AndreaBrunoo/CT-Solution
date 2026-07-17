using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend.Presentation;
using Frontend.Presentation.Service;
using Frontend.Application.Services;
using Frontend.Domain.Contracts;
using Frontend.Application.Extensions;
using Frontend.Infrastructure.Extensions;
using Frontend.Infrastructure.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5203/")
});

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5203/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WorkLogService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IApiClient, ApiClient>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

await builder.Build().RunAsync();