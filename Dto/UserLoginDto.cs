using System;
using System.Collections.Generic;

namespace MoodTrackerAPI_2.Dto
{
    /*
    * Author: William Wang
    * Email: wangwilliam2010@gmail.com
    * 
    * This DTO (Data Transfer Object) class represents user login information
    * within the MoodTrackerAPI application.
    *
    * Properties:
    * - LoginId: Unique identifier for the login record.
    * - UserId: Foreign key referencing the User entity.
    * - Username: Username associated with the user login.
    * - Password: Password for the user account.
    * - CreatedAt: Date and time when the login record was created (nullable).
    * - User: Reference to the related UserDto entity.
    *
    * This class is marked as partial, allowing for extension in other files
    * if additional functionality is needed without modifying the main class definition.
    */
    public partial class UserLoginDto
    {
        public int LoginId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public virtual UserDto User { get; set; } = null!;
    }
}
