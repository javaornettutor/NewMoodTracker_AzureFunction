using System;
using System.Collections.Generic;

namespace MoodTrackerAPI_2.Dto
{
    /*
    * Author: William Wang
    * Email: wangwilliam2010@gmail.com
    * 
    * UserDto class represents a Data Transfer Object for user information 
    * within the MoodTrackerAPI. It contains essential user properties 
    * such as UserId, Email, and CreatedAt, along with an IsAdmin flag 
    * to indicate administrative access.
    * 
    * Properties:
    * - UserId: Unique identifier for the user.
    * - Email: User's email address, must not be null.
    * - CreatedAt: Timestamp indicating when the user account was created.
    * - IsAdmin: Boolean flag indicating if the user has admin privileges.
    * - UserLogins: Collection of UserLoginDto objects representing user login records.
    */
    public partial class UserDto
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<UserLoginDto> UserLogins { get; set; } = new List<UserLoginDto>();
    }
}
