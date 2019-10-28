using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSpaceMicroservice.SignalRServices.Interfaces;

namespace DataSpaceMicroservice.HostedServices
{
    class ConsoleHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IApplicationLifetime appLifetime;

        private readonly IDataspaceHubClientService dataspaceHubClient;

        public ConsoleHostedService(
            IApplicationLifetime appLifetime,
            ILogger<ConsoleHostedService> logger,
            IDataspaceHubClientService dataspaceHubClient)
        {
            this.dataspaceHubClient = dataspaceHubClient;

            this.logger = logger;
            this.appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            logger.LogInformation("Starting Microservice (OnStarted)");

            try
            {
                // Connect to hubs
                dataspaceHubClient.Connect();
            }
            catch (Exception e)
            {
                logger.LogInformation("Microservice couldn't be started (OnStarted)");
                return;
            }
        }

        private void OnStopping()
        {
            logger.LogInformation("Stopping Microservice (OnStopping)");
        }

        private void OnStopped()
        {
            logger.LogInformation("Microservice stopped (OnStopped)");
        }
    }
}
