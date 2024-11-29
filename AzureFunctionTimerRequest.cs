using System;
using System.Data.SqlClient;
using System.Net.Mail;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewMoodTracker_AzureFunction.Models;
using SendGrid.Helpers.Mail;
using Serilog;
using Azure.Communication.Email;
using System;
using System.Threading.Tasks;


using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Azure;


namespace NewMoodTracker_AzureFunction
{
    public class AzureFunctionTimerRequest
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly AzureFunction_DBContext _context;
        
        public AzureFunctionTimerRequest(ILoggerFactory loggerFactory, AzureFunction_DBContext context)
        {
            _logger = loggerFactory.CreateLogger<AzureFunctionTimerRequest>();
            _context = context;
        }

        [Function("AzureFunctionTimerRequest")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await CheckDatabaseAndSendEmailAsync();
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
        private static async Task SendEmailAsyncViaAzure(string subject, string body)
        {
            string? azureCommunicationServiceKey = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");

            var client = new EmailClient(azureCommunicationServiceKey);
            
            string? emailFrom = Environment.GetEnvironmentVariable("SendGridEmailFrom");
            string? emailTo = Environment.GetEnvironmentVariable("SendGridEmailToEmail");

            var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@3e63d650-0222-429a-b681-3ee2cc4e1bfa.azurecomm.net",
                    content: new EmailContent("Test Email")
                    {
                        PlainText = "Hello world via email.",
                        Html = @"
		            <html>
			            <body>
				            <h1>Hello world via email.</h1>
			            </body>
		            </html>"
                    },
                    recipients: new EmailRecipients(new List<Azure.Communication.Email.EmailAddress> { new Azure.Communication.Email.EmailAddress(emailTo) })
            );

            try
            {
                EmailSendOperation emailSendOperation = client.Send(WaitUntil.Completed,emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
            
        private static async Task SendEmailWithSendGridAsync(string subject, string body)
        {
            string? apiKey = Environment.GetEnvironmentVariable("SendGridKey"); ;
            var client = new SendGrid.SendGridClient(apiKey);
            string ? emailFrom = Environment.GetEnvironmentVariable("SendGridEmailFrom");
            string? emailFromName= Environment.GetEnvironmentVariable("SendGridEmailFromName");
            string? emailTo= Environment.GetEnvironmentVariable("SendGridEmailToEmail");
            
            var from = new SendGrid.Helpers.Mail.EmailAddress(emailFrom, emailFromName);
            var to = new SendGrid.Helpers.Mail.EmailAddress(emailTo);

            

            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, to, subject, body, body);
            
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.StatusCode);
            // Optionally, print the response body for more details
            Console.WriteLine(await response.Body.ReadAsStringAsync());
        }

        private async Task CheckDatabaseAndSendEmailAsync()
        {
            bool hasNewEntries = await HasNewEntriesAsync();
            if (hasNewEntries)
            {
                _logger.LogInformation("New entries found. Sending email...");
                await SendEmailAsyncViaAzure("New Entries Notification", "New entries were added to the database.");

                //await SendEmailWithSendGridAsync("New Entries Notification", "New entries were added to the database.");
            }
            else
            {
                _logger.LogInformation("No new entries found.");
            }
        }

        private async Task<bool> HasNewEntriesAsync()
        {
            
            string connectionString = _context.Database.GetDbConnection().ConnectionString;
            string query = "SELECT COUNT(*) FROM UserComment WHERE DateCreated > DATEADD(MINUTE, -5, GETUTCDATE())";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int count = (int)await command.ExecuteScalarAsync();
                    _logger.LogInformation($"New entries count: {count}");
                    return count > 0;
                }
            }
        }
    }
}
