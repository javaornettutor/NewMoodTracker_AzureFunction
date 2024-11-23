using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewMoodTracker_AzureFunction.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.

builder.Services.AddDbContext<MoodTrackerContext>(options => options.UseSqlServer("Server=tcp:william.database.windows.net,1433;Database=MoodTracker;User Id=williamTest;Password=Pslord$1A3;Trust Server Certificate=True;"));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Build().Run();
