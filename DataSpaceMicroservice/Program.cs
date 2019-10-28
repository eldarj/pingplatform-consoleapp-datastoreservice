using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Mappers;
using DataSpaceMicroservice.Data.Services;
using DataSpaceMicroservice.Data.Services.Impl;
using DataSpaceMicroservice.HostedServices;
using DataSpaceMicroservice.RabbitMQ.Consumers;
using DataSpaceMicroservice.RabbitMQ.Consumers.Interfaces;
using DataSpaceMicroservice.SignalR.ClientServices;
using DataSpaceMicroservice.SignalRServices;
using DataSpaceMicroservice.SignalRServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ping.Commons.Settings;

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
                    services.AddDbContextPool<MyDbContext>(options =>
                    {
                        // TODO: Add this in appsettings or ENV (dev, prod) vars
                        options.UseLazyLoadingProxies()
                               .UseMySql("server=localhost;database=PingDataSpaceMicroserviceDb;user=root;password=", a =>
                                    a.MigrationsAssembly("DataSpaceMicroservice.Data"));
                    });

                    // Jwt authentication// configure strongly typed settings objects
                    services.Configure<SecuritySettings>(hostContext.Configuration.GetSection("SecuritySettings"));
                    services.Configure<GatewayBaseSettings>(hostContext.Configuration.GetSection("GatewayBaseSettings"));

                    services.AddHostedService<ConsoleHostedService>();
                    services.AddHostedService<AccountMQConsumer>();

                    services.AddTransient<IDataSpaceService, DataSpaceService>();
                    services.AddScoped<IAccountService, AccountService>();

                    services.AddScoped(provider => new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new DataspaceDtoMapperProfile(provider.GetService<MyDbContext>()));
                    }).CreateMapper());

                    services.AddScoped<IDataspaceHubClientService, DataspaceHubClientService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    configLogging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
