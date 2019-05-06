using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Channels;
using Ping.Commons.Dtos.Models.DataSpace;
using DataSpaceMicroservice.Data.Services;
using DataSpaceMicroservice.RabbitMQ.Consumers;
using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using Api.DtoModels.Auth;

namespace DataSpaceMicroservice.SignalR.ClientServices
{
    public class SignalRClientService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IApplicationLifetime appLifetime;
        private readonly IDataSpaceService dataSpaceService;
        private readonly IAccountService accountService;
        private readonly IAccountMQConsumer accountMQConsumer;

        private readonly HubConnection hubConnectionDataSpace;
        private readonly HubConnection hubConnectionAuth;

        public SignalRClientService(
            ILogger<SignalRClientService> logger,
            IApplicationLifetime applicationLifetime,
            IDataSpaceService dataSpaceService,
            IAccountService accountService,
            IAccountMQConsumer accountMQConsumer)
        {
            this.logger = logger;
            this.appLifetime = applicationLifetime;
            this.dataSpaceService = dataSpaceService;
            this.accountService = accountService;
            this.accountMQConsumer = accountMQConsumer;

            // Setup SignalR Hub connection
            hubConnectionDataSpace = new HubConnectionBuilder()
                .WithUrl("https://localhost:44380/dataspacehub?groupName=dataspaceMicroservice")
                .Build();

            hubConnectionAuth = new HubConnectionBuilder()
                .WithUrl("https://localhost:44380/authhub?groupName=dataspaceMicroservice")
                .Build();
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

        private async void OnStarted()
        {
            logger.LogInformation("Starting DataSpaceMicroservice (OnStarted)");

            // Connect to hub
            try
            {
                await hubConnectionDataSpace.StartAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.LogError("-- Couln't connect to signalR DataSpaceHub (OnStarted)");
                        return;
                    }
                    logger.LogInformation("DataSpaceMicroservice connected to DataSpaceHub successfully (OnStarted)");
                });

                await hubConnectionAuth.StartAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.LogError("-- Couln't connect to signalR AuthHub (OnStarted)");
                        return;
                    }
                    logger.LogInformation("DataSpaceMicroservice connected to AuthHub successfully (OnStarted)");
                });

                hubConnectionAuth.On<AccountDto>("RegistrationDone", async (accountDto) =>
                {
                    logger.LogInformation($"-- [AccountMicroservice] registered new account for {accountDto.PhoneNumber}.");

                    if(await accountService.CreateNewUser(accountDto))
                    {
                        logger.LogInformation($"-- {accountDto.PhoneNumber} account added to db. ");

                    }
                    else
                    {
                        logger.LogError($"-- Couldn't add {accountDto.PhoneNumber} account to db. ");
                    }
                });

                hubConnectionDataSpace.On<string, FileUploadDto>("SaveFileMetadata", async (appId, fileDto) =>
                {
                    logger.LogInformation($"-- {appId} requesting SaveFileMetadata for {appId}.");

                    // TODO: save file
                    FileUploadDto fileResponse = await dataSpaceService.FileUpload(fileDto);
                    if (fileResponse != null)
                    {
                        logger.LogInformation($"-- {fileResponse.FileName} metadata saved (Success). " +
                            $"Returning response dto obj.");
                        await hubConnectionDataSpace.SendAsync("SaveFileMetadataSuccess", appId, fileResponse);
                        return;
                    }

                    logger.LogError($"-- {fileResponse.FileName} metadata not saved (Fail). " +
                        $"Returning error message.");
                    await hubConnectionDataSpace.SendAsync("SaveFileMetadataFail", appId,
                        $"Saving metadata failed, for file: {fileDto}, requested by: {appId}");
                });
            }
            catch (Exception e)
            {
                logger.LogInformation("DataSpaceMicroservice couldn't be started (OnStarted)");
                return;
            }
            // Perform on-started activites here
        }

        private void OnStopping()
        {
            logger.LogInformation("Stopping DataSpaceMicroservice (OnStopping)");
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            logger.LogInformation("DataSpaceMicroservice stopped (OnStopped)");
            // Perform post-stopped activities here
        }
    }
}
