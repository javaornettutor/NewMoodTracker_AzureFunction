using AutoMapper;
using Azure;
using Grpc.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using MoodTrackerAPI_2.Dto;
using MoodTrackerAPI_2.Lib;
using NewMoodTracker_AzureFunction.Data;
using NewMoodTracker_AzureFunction.Models;
using System.Text.Json;
using Serilog;
using Serilog.Core;
using System.Net;
using ILogger = Serilog.ILogger;


namespace NewMoodTracker_AzureFunction
{
    public class MoodTrackerHttpRequestFunction
    {
        private ILogger _log;
        private readonly MoodTrackerContext _context; 
        private Validator validatorObj;
        private IMapper _mapper;
        public MoodTrackerHttpRequestFunction( IMapper mapper, MoodTrackerContext context)
        {
            _mapper = mapper;
            _context = context;
            validatorObj = new Validator(_context);    
        }

        public List<Mood> GetAllMoods()
        {
            var moods = _context.Moods.ToList();
            return moods; 
        }

       
        [Function("AveragePerMood")]
        public async Task<HttpResponseData> AveragePerMood([HttpTrigger(AuthorizationLevel.Admin, "get", Route = null)] HttpRequestData req, HttpResponseData response)
        {
            string intervalStr = req.Query["interval"];            
            string jsonResponse = string.Empty;

            if (string.IsNullOrEmpty(intervalStr))
            {
                getResponseObject(req, response, HttpStatusCode.BadRequest, "Please provide an interval");
                return response;
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
                        if (!moodCommentDic.ContainsKey(dictionKey))
                            moodCommentDic.Add(dictionKey, dictionVal);
                    }
                }
                jsonResponse = JsonSerializer.Serialize(moodCommentDic);
                getResponseObject(req, response, HttpStatusCode.OK, "", jsonResponse);
                return response;
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
                if (moodAverageDic.Count == 0)
                {
                    getResponseObject(req, response, HttpStatusCode.NotFound, "No record found");
                }
                else {
                    jsonResponse = JsonSerializer.Serialize(moodAverageDic);
                    getResponseObject(req, response, HttpStatusCode.OK, "", jsonResponse);
                }

                return response;
            }
            getResponseObject(req, response, HttpStatusCode.BadRequest, "Error");
            return response;
        }
        [Function("SubmitMood")]
        public async Task<HttpResponseData> SubmitMood([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, HttpResponseData response)
        {

            string curUserEmail = req.Query["curUserEmail"];


            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var userMood = Newtonsoft.Json.JsonConvert.DeserializeObject<UserMoodDto>(requestBody);

            if (userMood == null)
            {
                getResponseObject(req, response, HttpStatusCode.NotFound, "Unable to convert request body.");
                return response;
            }


            if (string.IsNullOrEmpty(curUserEmail))
            {
                getResponseObject(req, response, HttpStatusCode.NotFound, "Please provide your email");
                return response;
            }

            var curUserObj = validatorObj.GetUserByEmail(curUserEmail);
            User curUser = curUserObj.Result.Value;
            int curUserId = -1;

            if (curUser == null)
            {
                User newUser = new User();
                newUser.IsAdmin = false;
                newUser.Email = curUserEmail;
                _context.Users.Add(newUser);
                _context.SaveChanges();
                curUserId = newUser.UserId;
            }
            else
            {
                curUserId = curUser.UserId;
            }

            var duplicateForTheDay = validatorObj.IsMoodEnteredForToday(curUserId).Result;
            if (duplicateForTheDay)
            {
                getResponseObject(req, response, HttpStatusCode.Conflict, $"User [{curUserEmail}] already entered mood for the day");
                return response;
            }

            userMood.CreatedAt = DateTime.Now;
            userMood.UserId = curUserId;

            var userMoodDbObject = _mapper.Map<UserMood>(userMood);

            _context.UserMoods.AddAsync(userMoodDbObject);
            _context.SaveChangesAsync();
            getResponseObject(req, response, HttpStatusCode.Created, string.Empty);
            return response;
        }

        // Endpoint: GetAllUserMoodPerInterval - Retrieves user moods within a specified date interval.
        [Function("GetAllUserMoodPerInterval")]
        public async Task<HttpResponseData> GetAllUserMoodPerInterval([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = null)] HttpRequestData req, HttpResponseData response, ILogger log)
        {
            string jsonResponse= string.Empty;
            string intervalStr = req.Query["interval"];

            if (string.IsNullOrEmpty(intervalStr))
            {
                getResponseObject(req, response, HttpStatusCode.BadRequest, "invalid interval");
                return response;
            }
            int interval = int.Parse(intervalStr);

            if (interval > 0)
            {
                DateTime today = DateTime.Today;
                DateTime dateTo = today.AddDays(-interval);

                var result = _context.UserMoods.Where(ul => ul.CreatedAt.Value.Date <= today && ul.CreatedAt.Value.Date >= dateTo).ToList();
                jsonResponse = JsonSerializer.Serialize(result);
                getResponseObject(req, response, HttpStatusCode.OK, "", jsonResponse);

                return response;
            }
            else
            {
                getResponseObject(req, response, HttpStatusCode.BadRequest, "invalid interval");
                return response;
            }
        }


        public async Task<IActionResult> TestHttpTriggerFunction(
             [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "sample/{id?}")] HttpRequestData req, HttpResponseData response,
             ILogger log,
             string id)

        {


            // Retrieve query parameters
            string name = req.Query["name"];

            // Retrieve data from the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);

            // If "name" is not provided in query, check the request body
            name ??= data?.name;


            // Validate input
            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestObjectResult("Please provide a name in the query string or in the request body.");
            }

            // Return a response
            return new OkObjectResult($"Hello, {name}! Your ID is {id ?? "not provided"}.");
        }
        private void getResponseObject(HttpRequestData req, HttpResponseData response, HttpStatusCode statusCode, string errorMessage, string jsonObject="") 
        {
            if (response == null) { 
                response = req.CreateResponse();
            }
            response.StatusCode =  statusCode;
            
            if (!string.IsNullOrEmpty(errorMessage)) 
            { 
                response.WriteAsJsonAsync(new
                {                  
                    message = errorMessage
                });
            }
            if (!string.IsNullOrEmpty(jsonObject))
            {
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                response.WriteStringAsync(jsonObject);
             
            }           
        }       
    }
}
