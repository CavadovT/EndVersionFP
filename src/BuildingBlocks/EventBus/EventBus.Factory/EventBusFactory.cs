using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            //switch (config.EventBusType)
            //{
            //    case EventBusType.RabbitMQ:
            //        return new EventBusRabbitMQ(serviceProvider, config);
            //    case EventBusType.AzureServiceBus:
            //        return new EventBusServiceBus(serviceProvider, config);
            //    default:
            //        break;
            //}

            //or

            //return config.EventBusType switch
            //{
            //    EventBusType.AzureServiceBus => new EventBusServiceBus(config, serviceProvider),
            //    _ => new EventBusRabbitMQ(config, serviceProvider),
            //};


            return new EventBusRabbitMQ(config, serviceProvider);
        }
    }
}
