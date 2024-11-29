using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Data;
using NewMoodTracker_AzureFunction.Models;
using Newtonsoft.Json;

namespace NewMoodTracker_AzureFunction
{
    public class AzureFunctionHttpRequest
    {
        private readonly ILogger<AzureFunctionHttpRequest> _logger;
        private readonly AzureFunction_DBContext _context;
        public AzureFunctionHttpRequest(ILogger<AzureFunctionHttpRequest> logger, AzureFunction_DBContext context)
        {
            _context = context;
            _logger = logger;
        }

        [Function("FeedbackFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            // Use asynchronous method to read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            // Deserialize the request body to Feedback object
            var data = JsonConvert.DeserializeObject<Feedback>(requestBody);
            

            if (data == null || string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.Comment))
            {
                return new BadRequestObjectResult("Please provide a valid name and comment.");
            }

            // Simulate storing data (replace with Azure Table Storage or Cosmos DB logic)
            System.Console.WriteLine($"Received feedback from {data.Name}: {data.Comment}");
            _context.UserComments.Add(new UserComment() { UserName = data.Name, Comments = data.Comment });
            _context.SaveChanges();
            return new OkObjectResult("Feedback received successfully!");
        }

    }
}
