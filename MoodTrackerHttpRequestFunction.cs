using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Data;
using NewMoodTracker_AzureFunction.Models;

namespace NewMoodTracker_AzureFunction
{
    public class MoodTrackerHttpRequestFunction
    {
        private readonly ILogger<MoodTrackerHttpRequestFunction> _logger;
        private readonly MoodTrackerContext _context;

        public MoodTrackerHttpRequestFunction(ILogger<MoodTrackerHttpRequestFunction> logger)//, MoodTrackerContext context)
        {
            _logger = logger;
        //    _context = context;
        }

        [Function("HttpExampleFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
        //[Function("GetUserByEmail")]
        //public async Task<IActionResult> AddUser([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "users")] HttpRequest req, ILogger log)
        //{
        //    log.LogInformation("Processing AddUser request.");
        //    string usrEmail = req.Query["userEmail"];


        //    User? users = await _context.Users.SingleOrDefaultAsync(U => U.Email.Equals(usrEmail));


        //    return new OkObjectResult(users);
        //}
    }
}
