using System;
using System.Collections.Generic;

namespace NewMoodTracker_AzureFunction.Models;

public partial class Mood
{
    public int MoodId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserMood> UserMoods { get; set; } = new List<UserMood>();
}
