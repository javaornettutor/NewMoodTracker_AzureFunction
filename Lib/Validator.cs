using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MoodTrackerAPI_2.Dto;

using NewMoodTracker_AzureFunction.Data;
using NewMoodTracker_AzureFunction.Models;
using System.Text;

/*
* Author: William Wang
* Email: wangwilliam2010@gmail.com
* This class provides validation functions for the MoodTracker API.
* It interacts with the database context to verify data, focusing on
* user and mood record validation.
* 
* Methods:
* - GetUserByEmail: Checks if a user exists by their email address.
* - IsMoodEnteredForToday: Validates if a mood entry exists for the
*   specified user on the current day.
*/
namespace MoodTrackerAPI_2.Lib
{
    public class Validator
    {
        // Private field for the database context to interact with the data layer
        private readonly MoodTrackerContext _context;

        // Constructor accepting a database context for dependency injection
        public Validator(MoodTrackerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by email if they exist in the database.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>Returns a User object if found; otherwise, null.</returns>
        public async Task<ActionResult<User?>> GetUserByEmail(string email)
        {
            // Validate the incoming credentials
            User? userObj = await _context.Users.FirstOrDefaultAsync(ul => ul.Email.Trim().Equals(email.Trim()));
            return userObj;
        }

        /// <summary>
        /// Checks if a mood entry has already been recorded today for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>Returns true if a mood entry exists for today; otherwise, false.</returns>
        public async Task<bool> IsMoodEnteredForToday(int userId)
        {
            return _context.UserMoods.Where(u => u.CreatedAt.Value.Date == DateTime.Today.Date && u.UserId == userId).Count() > 0;
        }
    }
}
