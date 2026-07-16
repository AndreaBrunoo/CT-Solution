using Frontend.Services;
using Frontend;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

// HttpClient base
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5203") });

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WorkLogService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<DashboardService>();

await builder.Build().RunAsync();