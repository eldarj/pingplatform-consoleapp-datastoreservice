using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Text;
using DataSpaceMicroservice.RabbitMQ.Utils;
using Api.DtoModels.Auth;

namespace DataSpaceMicroservice.RabbitMQ.Consumers
{
    public class AccountMQConsumer : IAccountMQConsumer
    {

        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;
        private static Subscription _subscription;

        private readonly string ExchangeType = "fanout";
        private readonly string ExchangeName = "RegisterAccount_FanoutExchange";
        private readonly string QueueName = "DataSpaceMicroservice_RegisterAccount_Queue";

        public AccountMQConsumer()
        {
            CreateConnection();
        }

        public void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.ExchangeDeclare(exchange: ExchangeName,
                type: ExchangeType,
                durable: true,
                autoDelete: false,
                arguments: null);

            _model.QueueDeclare(queue: QueueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);

            _model.QueueBind(queue: QueueName, 
                exchange: ExchangeName, 
                routingKey: "");

            _model.BasicQos(0, 10, false);
            _subscription = new Subscription(_model, QueueName, false);
        }

        public void Close()
        {
            _connection.Close();
        }

        // We could call this upon instantiating the class (within the constructor), 
        // -- but we want to have the abillity to define the [T] param
        // -- so, we'll call ConsumeMessages right after we register this service, in Program.cs in this case
        public void ConsumeMessages()
        {
            Console.WriteLine("Listening for Topic <payment.purchaseorder>");
            Console.WriteLine("------------------------------------------");

            // TODO: change this to an OnEvent listener, so we don't run it constantly - we'll trigger our consume from SignalR or something
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = _subscription.Next();

                var message = (AccountDto) deliveryArguments.Body.Deserialize(typeof(AccountDto));
                var routingKey = deliveryArguments.RoutingKey;

                Console.WriteLine("RABBITMQ INFO: [Account Registered] - Message received from exchange/queue [{0}/{1}], data: {2}",
                    ExchangeName,
                    QueueName,
                    Encoding.Default.GetString(deliveryArguments.Body));

                _subscription.Ack(deliveryArguments);
            }

        }
    }
}