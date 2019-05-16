using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Mappers;
using DataSpaceMicroservice.Data.Services;
using DataSpaceMicroservice.Data.Services.Impl;
using DataSpaceMicroservice.RabbitMQ.Consumers;
using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using DataSpaceMicroservice.SignalR.ClientServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataSpaceMicroservice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("::DataSpace Microservice::");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: false);
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddDbContext<MyDbContext>();
                    services.AddDbContextPool<MyDbContext>(options => {
                        // TODO: Add this in appsettings or ENV (dev, prod) vars
                        options.UseMySql("server=localhost;database=PingDataSpaceMicroserviceDb;user=root;password=", 
                            a => a.MigrationsAssembly("DataSpaceMicroservice.Data"));
                    });

                    services.AddHostedService<SignalRClientService>();
                    services.AddScoped<IDataSpaceService, DataSpaceService>();
                    services.AddScoped<IAccountService, AccountService>();
                    services.AddScoped(provider => new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new DataspaceDtoMapperProfile(provider.GetService<MyDbContext>()));
                    }).CreateMapper());

                    services.AddScoped<IAccountMQConsumer, AccountMQConsumer>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
