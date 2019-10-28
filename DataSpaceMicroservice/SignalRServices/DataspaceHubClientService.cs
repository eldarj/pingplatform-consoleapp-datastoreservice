using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using DataSpaceMicroservice.SignalRServices.Interfaces;

using Ping.Commons.Settings;
using Ping.Commons.SignalR.Base;
using DataSpaceMicroservice.Data.Services;
using Ping.Commons.Dtos.Models.DataSpace;

namespace DataSpaceMicroservice.SignalRServices
{
    public class DataspaceHubClientService : BaseHubClientService, IDataspaceHubClientService
    {
        private static readonly string HUB_ENDPOINT = "dataspacehub";

        private readonly ILogger logger;
        private readonly IDataSpaceService dataSpaceService;

        public DataspaceHubClientService(IOptions<GatewayBaseSettings> gatewayBaseOptions,
            IOptions<SecuritySettings> securityOptions,
            IDataSpaceService dataSpaceService,
            ILogger<DataspaceHubClientService> logger)
            : base(gatewayBaseOptions, securityOptions, HUB_ENDPOINT)
        {
            this.logger = logger;
            this.dataSpaceService = dataSpaceService;
        }

        public async void Connect()
        {
            await hubConnection.StartAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    logger.LogInformation("-- Couln't connect to SignalR DataspaceHub (OnStarted)");
                    return;
                }
                logger.LogInformation("DataSpaceMicroservice connected to DataspaceHub successfully (OnStarted)");
                RegisterHandlers();
            });
        }

        public void RegisterHandlers()
        {
            hubConnection.On<string, DirectoryDto>("SaveDirectoryMetadata", async (phonenumber, dirDto) =>
            {
                logger.LogInformation($"-- {phonenumber} requesting SaveDirectoryMetadata for {phonenumber}.");

                // TODO: save dir
                NodeDto directoryResponse = await dataSpaceService.NewDirectory(phonenumber, dirDto);
                if (directoryResponse != null)
                {
                    logger.LogInformation($"-- {directoryResponse.Name} metadata saved (Success). " +
                        $"Returning response dto obj.");
                    await hubConnection.SendAsync("SaveDirectoryMetadataSuccess", phonenumber, directoryResponse);
                    return;
                }

                logger.LogError($"-- {directoryResponse.Name} metadata not saved (Fail). " +
                    $"Returning error message.");
                await hubConnection.SendAsync("SaveDirectoryMetadataFail", phonenumber,
                    $"Saving metadata failed, for file: {dirDto}, requested by: {phonenumber}");
            });

            hubConnection.On<string, FileDto>("SaveFileMetadata", async (phonenumber, fileDto) =>
            {
                logger.LogInformation($"-- {phonenumber} requesting SaveFileMetadata for {phonenumber}.");

                // TODO: save file
                NodeDto fileResponse = dataSpaceService.FileUpload(phonenumber, fileDto);
                if (fileResponse != null)
                {
                    logger.LogInformation($"-- {fileResponse.Name} metadata saved (Success). " +
                        $"Returning response dto obj.");
                    await hubConnection.SendAsync("SaveFileMetadataSuccess", phonenumber, fileResponse);
                    return;
                }

                logger.LogError($"-- {fileResponse.Name} metadata not saved (Fail). " +
                    $"Returning error message.");
                await hubConnection.SendAsync("SaveFileMetadataFail", phonenumber,
                    $"Saving metadata failed, for file: {fileDto}, requested by: {phonenumber}");
            });

            hubConnection.On<string>("RequestFilesMetaData", async (phoneNumber) =>
            {
                logger.LogInformation($"-- {phoneNumber} requesting FilesMetaData for account: {phoneNumber}.");

                // TODO: get list of all dirs and files (metadata)
                DataSpaceMetadata dataSpaceMetadata = await dataSpaceService.GetAllByOwner(phoneNumber);
                if (dataSpaceMetadata != null)
                {
                    logger.LogInformation($"-- Returning metadata for all dirs/files.");
                    await hubConnection.SendAsync("RequestFilesMetaDataSuccess", phoneNumber, dataSpaceMetadata);
                    return;
                }

                logger.LogError($"-- Request couldn't be executed - returning error message.");
                await hubConnection.SendAsync("RequestFilesMetaDataFail", phoneNumber,
                    $"Couldn't load directories and files, for account by number: {phoneNumber}, requested by: {phoneNumber}");
            });

            hubConnection.On<string, string>("DeleteFileMetadata", async (phoneNumber, filePath) =>
            {
                logger.LogInformation($"-- {phoneNumber} requesting DeleteFileMetadata ('{filePath}') by account: {phoneNumber}.");

                // TODO: delete file and return appropriate result
                if (await dataSpaceService.DeleteFile(phoneNumber, filePath))
                {
                    logger.LogInformation($"-- File deleted successfully.");
                    await hubConnection.SendAsync("DeleteFileMetadataSuccess", phoneNumber, filePath);
                    return;
                }

                logger.LogError($"-- Request couldn't be executed- returning error message.");
                await hubConnection.SendAsync("DeleteFileMetadataFail", phoneNumber, filePath,
                    $"Couldn't find {filePath} for owner: {phoneNumber}, requested by: {phoneNumber}");
            });

            hubConnection.On<string, string>("DeleteDirectoryMetadata", async (phoneNumber, directoryPath) =>
            {
                logger.LogInformation($"-- {phoneNumber} requesting DeleteDirectoryMetadata ('{directoryPath}') by account: {phoneNumber}.");

                // TODO: delete directory and return appropriate result
                if (await dataSpaceService.DeleteDirectory(phoneNumber, directoryPath))
                {
                    logger.LogInformation($"-- Directory deleted successfully.");
                    await hubConnection.SendAsync("DeleteDirectoryMetadataSuccess", phoneNumber, directoryPath);
                    return;
                }

                logger.LogError($"-- Request couldn't be executed- returning error message.");
                await hubConnection.SendAsync("DeleteDirectoryMetadataFail", phoneNumber, directoryPath,
                    $"Couldn't find directory:{directoryPath} for owner: {phoneNumber}, requested by: {phoneNumber}");
            });

            hubConnection.On<string, List<SimpleNodeDto>>("DeleteMultipleNodesMetadata", async (phoneNumber, nodes) =>
            {
                logger.LogInformation($"-- {phoneNumber} requesting DeleteMultipleNodesMetadata (total {nodes.Count}) by account: {phoneNumber}.");

                // TODO: delete directory and return appropriate result
                if (await dataSpaceService.BatchDeleteNodes(phoneNumber, nodes))
                {
                    logger.LogInformation($"-- Nodes batch deleted successfully.");
                    await hubConnection.SendAsync("DeleteMultipleNodesMetadataSuccess", phoneNumber, nodes);
                    return;
                }

                logger.LogError($"-- Request couldn't be executed - returning error message.");
                await hubConnection.SendAsync("DeleteMultipleNodesMetadataFail", phoneNumber, nodes,
                    $"Nodes batch delete failed (total {nodes.Count}) for owner: {phoneNumber}, requested by: {phoneNumber}");
            });
        }
    }
}
