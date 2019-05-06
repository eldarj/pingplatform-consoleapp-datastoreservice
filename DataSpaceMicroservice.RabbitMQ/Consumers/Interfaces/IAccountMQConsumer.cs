using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces
{
    public interface IAccountMQConsumer
    {
        void CreateConnection();

        void Close();

        void ConsumeMessages<T>(T account);
    }
}
