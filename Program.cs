using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.

// Configure logging
builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole();     // Add Console logging
builder.Logging.AddDebug();       // Add Debug logging

builder.Services.AddDbContext<MoodTrackerContext>(options => options.UseSqlServer("Server=tcp:william.database.windows.net,1433;Database=MoodTracker;User Id=williamTest;Password=Pslord$1A3;Trust Server Certificate=True;"));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Build().Run();
