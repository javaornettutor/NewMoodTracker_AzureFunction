using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoodTrackerAPI_2.Dto
{
    /*
     * Author: William Wang
     * Email: wangwilliam2010@gmail.com
     *
     * MoodDto class is a Data Transfer Object (DTO) for representing mood-related data
     * in the Mood Tracker API. It contains the following properties:
     * 
     * - MoodId: Unique identifier for each mood entry.
     * - Description: A string description of the mood, which cannot be null.
     * - CreatedAt: Date and time the mood entry was created, optional.
     */
    public partial class MoodDto
    {
        public int MoodId { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}
