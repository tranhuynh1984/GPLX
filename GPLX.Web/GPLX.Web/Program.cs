using GPLX.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace GPLX.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
          

            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetService<ILogger<Program>>();
            try
            {
                //#if DEBUG
                CreateDbIfNotExists(builder);
                //#endif
                logger.LogInformation("Starting web host");
                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host unexpectedly terminated");
            }
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<Context>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, configBuilder) =>
                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                )
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                });
    }
}
