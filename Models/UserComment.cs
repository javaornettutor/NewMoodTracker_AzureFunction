using System;
using System.Collections.Generic;

namespace NewMoodTracker_AzureFunction.Models;

public partial class UserComment
{
    public int CommentId { get; set; }

    public string UserName { get; set; } = null!;

    public string Comments { get; set; } = null!;

    public DateTime? DateCreated { get; set; }
}
