using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights;
using System;

using Microsoft.EntityFrameworkCore;
using FluentAssertions.Common;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

[assembly: FunctionsStartup(typeof(MyFunctionApp.Program))]

namespace MyFunctionApp
{
    public class Program
    {
        private static string instrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
        private static string instrumentation_Conn_Str = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATION_CONN_STR");
        private static string DB_Conn_Str = Environment.GetEnvironmentVariable("SqlConnectionString");

        public static void Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.ConfigureFunctionsWebApplication();

            builder.Services.AddDbContext<MoodTrackerContext>(options => options.UseSqlServer(DB_Conn_Str));
            builder.Services.AddAutoMapper(typeof(Program).Assembly);


            // Build and run the host
            Host.CreateDefaultBuilder(args)
               .ConfigureLogging((context, logging) =>
               {
                   // Clear default providers and add Serilog
                   logging.ClearProviders();
                   logging.AddSerilog(); // Integrate Serilog into Azure Functions logging
               });

            builder.Build().Run();
        }

        
           
    }
}
