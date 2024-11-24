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
//        private static string instrumentationKey = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_INSTRUMENTATIONKEY");
//        private static string instrumentation_Conn_Str = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATION_CONN_STR");

        
        public static void Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.ConfigureFunctionsWebApplication();

            builder.Services.AddDbContext<MoodTrackerContext>(options => options.UseSqlServer("Server=tcp:william.database.windows.net,1433;Database=MoodTracker;User Id=williamTest;Password=Pslord$1A3;Trust Server Certificate=True;"));
            builder.Services.AddAutoMapper(typeof(Program).Assembly);


            //builder.Services.AddSingleton<TelemetryClient>(provider =>
            //{
            //    var config = TelemetryConfiguration.CreateDefault();
            //    config.InstrumentationKey = instrumentation_Conn_Str;
            //    return new TelemetryClient(config);
            //});

            // Optionally, configure Application Insights for logging
        //    builder.Services.AddApplicationInsightsTelemetry(instrumentationKey);
            

            //builder.Services.AddApplicationInsightsTelemetry(instrumentation_Conn_Str);

            //builder.Services.AddSingleton<ITelemetryModule, DependencyTrackingTelemetryModule>();

            // Build and run the host
            Host.CreateDefaultBuilder(args)
               .ConfigureLogging((context, logging) =>
               {
                   // Clear default providers and add Serilog
                   logging.ClearProviders();
                   logging.AddSerilog(); // Integrate Serilog into Azure Functions logging
               });

            //Log.Logger = new LoggerConfiguration()
            //   .MinimumLevel.Debug() // Set the minimum logging level
            //   .WriteTo.ApplicationInsights(instrumentationKey, TelemetryConverter.Traces) // Configure Application Insights sink
            //   .CreateLogger();
            builder.Build().Run();
        }

        
           
    }
}
