using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces
{
    public interface IAccountMQConsumer
    {
        Task CreateConnection();

        Task Close();

        void OnDeliveryReceived(object model, BasicDeliverEventArgs delivery);
    }
}
