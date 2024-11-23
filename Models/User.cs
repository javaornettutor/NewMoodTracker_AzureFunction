using System;
using System.Collections.Generic;

namespace NewMoodTracker_AzureFunction.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool IsAdmin { get; set; }

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
