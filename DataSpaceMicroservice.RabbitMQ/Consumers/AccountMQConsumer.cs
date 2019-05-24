using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using DataSpaceMicroservice.RabbitMQ.Utils;
using Api.DtoModels.Auth;
using Microsoft.Extensions.Hosting;
using DataSpaceMicroservice.Data.Context;
using Microsoft.Extensions.Logging;
using DataSpaceMicroservice.Data.Services;
using System.Threading.Tasks;
using System.Threading;

namespace DataSpaceMicroservice.RabbitMQ.Consumers
{
    public class AccountMQConsumer : IHostedService, IAccountMQConsumer
    {
        private readonly string ExchangeType = "fanout";
        private readonly string ExchangeName = "RegisterAccount_FanoutExchange";
        private readonly string QueueName = "DataSpaceMicroservice_RegisterAccount_Queue";

        private static ConnectionFactory connectionFactory;
        private static IConnection connection;
        private static IModel channel;
        private static EventingBasicConsumer consumer;

        private readonly MyDbContext dbContext;
        private readonly IAccountService accountService;
        private readonly ILogger logger;

        public AccountMQConsumer(MyDbContext dbContext, IAccountService accountService, ILogger<AccountMQConsumer> logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.accountService = accountService;
        }

        public Task StartAsync(CancellationToken cancellationToken) => CreateConnection();
        public Task StopAsync(CancellationToken cancellationToken) => Close();

        public Task CreateConnection()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("- Creating connection to RabbitMQ...");
                connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                connection = connectionFactory.CreateConnection();
                channel = connection.CreateModel();

                // Define and bind the exchange and the queue
                channel.ExchangeDeclare(exchange: ExchangeName,
                    type: ExchangeType,
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                channel.QueueDeclare(queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(queue: QueueName,
                    exchange: ExchangeName,
                    routingKey: "");

                // Bind a consumer with our OnDeliveryReceived handler
                consumer = new EventingBasicConsumer(channel);
                consumer.Received += OnDeliveryReceived;

                // Start basic consuming on our channel, with auto-acknowledgement
                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            });
        }

        public async void OnDeliveryReceived(object model, BasicDeliverEventArgs delivery)
        {
            Console.WriteLine($"Consuming data from RabbitMQ. Exchange: {ExchangeName} - Qeueue: {QueueName}");
            Console.WriteLine("------------------------------------------");

            var messageBody = delivery.Body;
            var accountDto = (AccountDto) messageBody.Deserialize(typeof(AccountDto));

            Console.WriteLine(" [x] RABBITMQ INFO: [Account Registered] - Message received from exchange/queue [{0}/{1}], data: {2}",
                ExchangeName,
                QueueName,
                Encoding.UTF8.GetString(messageBody));

            if (await accountService.CreateNewUser(accountDto))
            {
                logger.LogInformation($"-- Account: {accountDto.PhoneNumber} added to db. ");
            }
            else
            {
                logger.LogError($"-- Couldn't add Account: {accountDto.PhoneNumber} to db. ");
            }
        }

        public Task Close()
        {
            connection.Close();
            return Task.CompletedTask;
        }
    }
}