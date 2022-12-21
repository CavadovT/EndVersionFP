using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using RabbitMQ.Client;
using Serilog;



//string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
ServiceCollection services = new ServiceCollection();

ConfigureServices(services);
//services
//    .AddLogging(x => new LoggerConfiguration()
//.ReadFrom.Configuration()
//.CreateLogger()
//);
var sp = services.BuildServiceProvider();
IEventBus eventBus = sp.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Log.Logger.Information("Application is Running....");

Console.ReadKey();


void ConfigureServices(ServiceCollection services)
{
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();


    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventBusType = EventBusType.RabbitMQ,
            Connection = new ConnectionFactory()
            {
                HostName = "c_rabbitmq"
            }
        };
        return EventBusFactory.Create(config, sp);
    });
}

