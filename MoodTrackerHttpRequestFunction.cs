using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MoodTrackerAPI_2.Dto;
using MoodTrackerAPI_2.Lib;
using NewMoodTracker_AzureFunction.Data;
using NewMoodTracker_AzureFunction.Models;
using Newtonsoft.Json;
using System.Net;

namespace NewMoodTracker_AzureFunction
{
    public class MoodTrackerHttpRequestFunction
    {
        private readonly ILogger<MoodTrackerHttpRequestFunction> _logger;
        private readonly MoodTrackerContext _context; 
        private Validator validatorObj;
        
        public MoodTrackerHttpRequestFunction( MoodTrackerContext context, ILogger<MoodTrackerHttpRequestFunction> logger)
        {
            _logger = logger;
            _context = context;
            validatorObj = new Validator(_context);
        }

        [Function("HttpExampleFunction")]
        public IActionResult HttpExampleFunction([HttpTrigger(AuthorizationLevel.User, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
        
        
        [Function("GetUserByEmail")]
        public IActionResult GetUserByEmail([HttpTrigger(AuthorizationLevel.Function, "GET", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Processing AddUser request.");
            string usrEmail = req.Query["userEmail"];
           
            // log.LogInformation("Test");
            var user = _context.Users.SingleOrDefault(U => U.Email.Equals(usrEmail));
            if (string.IsNullOrEmpty(usrEmail) || user==null)
            {
                return new NotFoundObjectResult("No user with email ["+ usrEmail  + "] found.");
            }

            return new OkObjectResult(user);
        }

       
        public List<Mood> GetAllMoods()
        {
            var moods = _context.Moods.ToList();
            //Console.WriteLine(moods);
            return moods; // 200 is OK status code
        }
        // Endpoint: GetAllUserMoodPerInterval - Retrieves user moods within a specified date interval.
        [Function("GetAllUserMoodPerInterval")]
        public IActionResult GetAllUserMoodPerInterval([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = null)] HttpRequest req, ILogger log)
        {
           
            string intervalStr = req.Query["interval"];

            if (string.IsNullOrEmpty(intervalStr)) 
            {
                return new NotFoundObjectResult("invalid interval");
            }
            int interval = int.Parse(intervalStr);

            if (interval > 0)
            {
                DateTime today = DateTime.Today;
                DateTime dateTo = today.AddDays(-interval);

                return new OkObjectResult(_context.UserMoods.Where(ul => ul.CreatedAt.Value.Date <= today && ul.CreatedAt.Value.Date >= dateTo).ToList());
            }
            else {
                return new NotFoundObjectResult("invalid interval");
            }
            
           
        }

        // Endpoint: AveragePerMood - Calculates average mood rating or returns mood comments for a given interval.
        [Function("AveragePerMood")]
        public IActionResult AveragePerMood([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = null)] HttpRequest req, ILogger log)
        {
            string intervalStr = req.Query["interval"];


            if (string.IsNullOrEmpty(intervalStr))
            {
                return new NotFoundObjectResult("invalid interval");
            }
            int interval = int.Parse(intervalStr);


            if (interval == -1)
            {
                Dictionary<string, string> moodCommentDic = new Dictionary<string, string>();
                List<UserMood> userMoodWithComment = _context.UserMoods.Where(u => u.MoodComments != null && u.MoodComments.Length > 0).OrderByDescending(u => u.CreatedAt).ToList();

                foreach (UserMood usrMoodItem in userMoodWithComment)
                {
                    User? curUser = _context.Users.SingleOrDefault(u => u.UserId == usrMoodItem.UserId);
                    string dictionKey = string.Empty;
                    string dictionVal = string.Empty;

                    if (curUser != null && !string.IsNullOrEmpty(usrMoodItem.MoodComments))
                    {
                        dictionKey = curUser.Email;
                        string dictValFormat = "[{0}]\nPost on :[{1}]";
                        dictionVal = string.Format(dictValFormat, usrMoodItem.MoodComments, usrMoodItem.CreatedAt.Value);
                        if(!moodCommentDic.ContainsKey(dictionKey))
                            moodCommentDic.Add(dictionKey, dictionVal);
                    }
                }
                return new OkObjectResult(moodCommentDic);
            }
            else if (interval > 0)
            {
                DateTime today = DateTime.Today;
                DateTime dateTo = today.AddDays(-interval);
                
                List<Mood> allMood = _context.Moods.ToList();
            
                Dictionary<string, float> moodAverageDic = new Dictionary<string, float>();

                foreach (Mood eachMood in allMood)
                {
                    List<UserMood> result2 = _context.UserMoods.Where(ul => ul.CreatedAt.Value.Date <= today && ul.CreatedAt.Value.Date >= dateTo).ToList();
                    int totalPerMood = result2.Count();
                    int curMoodCount = result2.Where(u => u.MoodType == eachMood.MoodId).Count();
                    float average = (float)curMoodCount / totalPerMood;

                    if (!float.IsNaN(average))
                        moodAverageDic.Add(eachMood.Description, (float)Math.Round(average, 2));
                }
                return new OkObjectResult(moodAverageDic.Count == 0 ? null : moodAverageDic);
            }

            return new NotFoundObjectResult("error"); 
        }
        
        // Endpoint: SubmitMood - Allows users to submit a mood entry for the day.
        //[HttpPost("SubmitMood")]
        //public async IActionResult SubmitMood([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        //{

        //    string curUserEmail = req.Query["curUserEmail"];

            
        //    if (string.IsNullOrEmpty(curUserEmail))
        //    {
        //        return new NotFoundObjectResult("please provide an email");
        //    }

        //    var curUserObj = await validatorObj.GetUserByEmail(curUserEmail);
        //    User curUser = curUserObj.Value;
        //    int curUserId = -1;

        //    if (curUser == null)
        //    {
        //        User newUser = new User();
        //        newUser.IsAdmin = false;
        //        newUser.Email = curUserEmail;
        //        _context.Users.Add(newUser);
        //        _context.SaveChanges();
        //        curUserId = newUser.UserId;
        //    }
        //    else
        //    {
        //        curUserId = curUser.UserId;
        //    }

        //    var duplicateForTheDay = validatorObj.IsMoodEnteredForToday(curUserId).Result;
        //    if (duplicateForTheDay)
        //    {
        //        return new NotFoundObjectResult($"User [{curUserEmail}] already entered mood for the day");
        //    }

        //    userMood.CreatedAt = DateTime.Now;
        //    userMood.UserId = curUserId;

        //    var userMoodDbObject = _mapper.Map<UserMood>(userMood);

        //    await _context.UserMoods.AddAsync(userMoodDbObject);
        //    await _context.SaveChangesAsync();
        //    return new OkResult();
        //}
    }
}
