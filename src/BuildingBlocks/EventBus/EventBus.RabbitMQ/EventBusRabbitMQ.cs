using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        private readonly RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel consumerChannel;

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            //if (config.Connection != null)
            //{
            //    var connectJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
            //    {
            //        //Self referencing loop detected for property
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    });

            //    connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectJson);

            //}
            if (config.Connection!=null)
            {
                connectionFactory = new ConnectionFactory()
                {
                    HostName= "c_rabbitmq"
                };
            }
            else
            {
                connectionFactory = new ConnectionFactory();
            }
            
            persistentConnection = new RabbitMQPersistentConnection(connectionFactory, config.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    //if exception can be then =>logging

                });

            var eventName = @event.GetType().Name;

            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");//Ensure exchange exists while publishing

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();

                properties.DeliveryMode = 2;// persistent

                //consumerChannel.QueueDeclare(
                //    queue: GetSubName(eventName),//Ensure Queue exists while publishing
                //    exclusive: false,
                //    durable: true,
                //    autoDelete: false,
                //    arguments: null
                //    );

                //consumerChannel.QueueBind(
                //    queue: GetSubName(eventName),
                //    exchange: EventBusConfig.DefaultTopicName,
                //    routingKey: eventName);

                consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                    );
            });

        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(
                    queue: GetSubName(eventName),//Ensure queue exists while consuming
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                consumerChannel.QueueBind(
                    queue: GetSubName(eventName),
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName);

            }
            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void Unsubscribe<T, TH>()

        {
            SubsManager.RemoveSubscription<T, TH>();
        }

        private void SubsManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();

            }

            consumerChannel.QueueUnbind(
                queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName
                );

            if (SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }


        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer
                    );

            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);

            var message = Encoding.UTF8.GetString(e.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception)
            {
                //logging
            }

            consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }
}
