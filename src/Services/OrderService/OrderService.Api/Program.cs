using ConsulRegistration;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.OpenApi.Models;
using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration = builder.Configuration;
IWebHostEnvironment env = builder.Environment;
builder.WebHost.UseDefaultServiceProvider((context, opt) =>
{
    opt.ValidateOnBuild = false;
    opt.ValidateScopes = false;

});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService.Api", Version = "v1" });

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

builder
    .Logging
    .Services
    .AddLogging(x => new LoggerConfiguration()
.ReadFrom.Configuration(Configuration.AddSerilogConfiguration(env))
.CreateLogger()
);

builder.Services
    .AddApplicationRegistration(typeof(Program))
    .AddPersistenceRegistration(builder.Configuration)
    .ConfigureEventHandlers()
    .ConfigureConsul(builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "OrderService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = "c_rabbitmq"
        }
    };

    return EventBusFactory.Create(config, sp);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MigrateDbContext<OrderDbContext>((context, services) =>
{

    var env = services.GetService<IWebHostEnvironment>();
    var logger = services.GetService<ILogger<OrderDbContext>>();

    new OrderDbContextSeed()
            .SeedAsync(context, logger)
            .Wait();

});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.RegisterWithConsul(app.Lifetime, app.Configuration);

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
