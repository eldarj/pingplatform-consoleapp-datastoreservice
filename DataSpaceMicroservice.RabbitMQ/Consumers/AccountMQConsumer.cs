using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Text;
using DataSpaceMicroservice.RabbitMQ.Utils;

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

        public void ConsumeMessages<T>(T account)
        {
            Console.WriteLine("Listening for Topic <payment.purchaseorder>");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine();

            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = _subscription.Next();

                var message = (T)deliveryArguments.Body.Deserialize(typeof(T));
                var routingKey = deliveryArguments.RoutingKey;

                Console.WriteLine("RABBITMQ INFO: [Account Registered] - Message received from exchange/queue [{0}/{1}], data: {2}",
                    ExchangeName,
                    QueueName,
                    deliveryArguments);

                _subscription.Ack(deliveryArguments);
            }

        }
    }
}