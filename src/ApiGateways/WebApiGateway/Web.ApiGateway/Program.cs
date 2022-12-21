using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Web.ApiGateway.Infrastructure;
using Web.ApiGateway.Interfaces;
using Web.ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web.ApiGateway", Version = "v1" });
});

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
    .AddJsonFile("ocelot.json")
    .AddEnvironmentVariables();
});
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", b =>
    {
        b.SetIsOriginAllowed((hst) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});//AddCorPolicy Errror handle
builder.Services.AddOcelot().AddConsul();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient("basket", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["urls:basket"]);
})
    .AddHttpMessageHandler<HttpClientDelegatingHandler>();

builder.Services.AddHttpClient("catalog", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["urls:catalog"]);
})
    .AddHttpMessageHandler<HttpClientDelegatingHandler>();

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IBasketService, BasketService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web.ApiGateway v1"));

}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseOcelot();

app.Run();
