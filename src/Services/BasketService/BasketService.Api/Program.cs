using Autofac;
using Autofac.Extensions.DependencyInjection;
using BasketService.Api.AutoFac;
using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using ConsulRegistration;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacModule());
    });

IConfiguration Configuration = builder.Configuration;
IWebHostEnvironment env = builder.Environment;

builder.WebHost.UseDefaultServiceProvider((context, opt) =>
{
    opt.ValidateOnBuild = false;
    opt.ValidateScopes = false;

});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasketService.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                     },
                     Array.Empty<string>()
                   }
                });
});


builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(configure => { configure.AddConsole(); configure.SetMinimumLevel(LogLevel.Debug); });

builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddSingleton<IEventBus>(serprovider =>
{

    EventBusConfig config = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory
        {
            HostName = "c_rabbitmq",
        }
    };

    return EventBusFactory.Create(config, serprovider);
});

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();//depend inject=>> call handle method in event handler


builder
    .Logging
    .Services
    .AddLogging(x => new LoggerConfiguration()
.ReadFrom.Configuration(Configuration.AddSerilogConfiguration(env))
.CreateLogger()
);


var app = builder.Build();

app.Logger.LogInformation("System app and running - From Configuration {TestParam}", "Tural Cavadov");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketService.Api v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.RegisterWithConsul(app.Lifetime, app.Configuration);


IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
