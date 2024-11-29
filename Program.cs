using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Data;
using Microsoft.Azure.Functions.Worker.Extensions.Http;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights;
using System;
using Microsoft.EntityFrameworkCore;
using FluentAssertions.Common;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Google.Protobuf.WellKnownTypes;
using NewMoodTracker_AzureFunction.Models;

[assembly: FunctionsStartup(typeof(MyFunctionApp.Program))]

namespace MyFunctionApp
{
    public class Program
    {
        private static string instrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
        private static string instrumentation_Conn_Str = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATION_CONN_STR");
        private static string MoodTracker_DB_Conn_Str = Environment.GetEnvironmentVariable("MoodTracker_SqlConnectionString");
        private static string AzureFunction_DB_Conn_Str = Environment.GetEnvironmentVariable("AzureFunction_DB_SqlConnectionString");
        

        public static void Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.ConfigureFunctionsWebApplication();

            builder.Services.AddDbContext<MoodTrackerContext>(options => options.UseSqlServer(MoodTracker_DB_Conn_Str));
            builder.Services.AddDbContext<AzureFunction_DBContext>(options => options.UseSqlServer(AzureFunction_DB_Conn_Str));
            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            
            var allowAllOrigins = "CorsAllowAllOrigins";
            // Add services to the container
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowAllOrigins) // Specify allowed origins
                          .AllowAnyHeader()                  // Allow any headers
                          .AllowAnyMethod();                 // Allow any HTTP methods
                });
            });

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
