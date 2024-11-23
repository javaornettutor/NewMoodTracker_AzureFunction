using System;
using System.Collections.Generic;

namespace NewMoodTracker_AzureFunction.Models;

public partial class UserLogin
{
    public int LoginId { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
