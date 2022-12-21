using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebUI;
using WebUI.Application.Interfaces;
using WebUI.Application.Services;
using WebUI.Infrastructure;
using WebUI.Utils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<ICatalogService, CatalogService>();
builder.Services.AddTransient<IBasketService, BasketService>();
builder.Services.AddTransient<IOrderService, OrderService>();

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddSingleton<AppStateManager>();

builder.Services.AddScoped(serProvider =>
{
    var clientFactory = serProvider.GetRequiredService<IHttpClientFactory>();

    return clientFactory.CreateClient("ApiGatewayHttpClient");
});

builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddHttpClient("ApiGatewayHttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
})
    .AddHttpMessageHandler<AuthTokenHandler>();


await builder.Build().RunAsync();