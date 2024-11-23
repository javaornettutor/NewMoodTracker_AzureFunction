using System;
using System.Collections.Generic;

namespace NewMoodTracker_AzureFunction.Models;

public partial class UserMood
{
    public int MoodId { get; set; }

    public int UserId { get; set; }

    public int MoodType { get; set; }

    public string? MoodComments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Mood MoodTypeNavigation { get; set; } = null!;
}
