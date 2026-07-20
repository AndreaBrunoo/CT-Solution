using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend.Presentation;
using Frontend.Presentation.Service;
using Frontend.Application.Extensions;
using Frontend.Infrastructure.Extensions;
using Frontend.Domain.Contracts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

await builder.Build().RunAsync();