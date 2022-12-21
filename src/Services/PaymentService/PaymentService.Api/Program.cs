using ConsulRegistration;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using PaymentService.Api.Registrations;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;
IConfiguration Configuration = builder.Configuration;

builder.WebHost.UseDefaultServiceProvider((context, opt) =>
{
    opt.ValidateOnBuild = false;
    opt.ValidateScopes = false;
});

//Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.ConfigureConsul(builder.Configuration);
//add services
builder
    .Logging
    .Services
    .AddLogging(x => new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration.AddSerilogConfiguration(env))
.CreateLogger()
);


builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();//depend inject=>> call handle method in event handler

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "PaymentService",
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

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.RegisterWithConsul(app.Lifetime,app.Configuration);

app.MapControllers();

IEventBus eventBus =app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();


app.Run();
