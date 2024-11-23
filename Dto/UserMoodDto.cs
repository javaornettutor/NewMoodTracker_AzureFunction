// Importing necessary namespaces for the MoodTracker API
   // Contains shared libraries and utilities used across the application
   // Contains models representing data structures in the application
using System;                     // Provides fundamental classes and base classes that define commonly-used values and data types
using System.Collections.Generic; // Provides classes to define collections of objects

namespace MoodTrackerAPI_2.Dto
{
    /*
    * Author: William Wang
    * Email: wangwilliam2010@gmail.com
    * 
    * This Data Transfer Object (DTO) represents the mood data for a user.
    * It is used to transfer mood information such as mood type, comments,
    * and timestamps between layers of the application without exposing
    * underlying database entities directly.
    */
    public partial class UserMoodDto
    {
        public int MoodId { get; set; }             // Unique identifier for the user's mood entry
        public int UserId { get; set; }             // Identifier for the user associated with this mood entry
        public int MoodType { get; set; }           // Type of mood, represented by an integer (e.g., 1 = happy, 2 = sad, etc.)
        public string? MoodComments { get; set; }   // Optional comments about the user's mood
        public DateTime? CreatedAt { get; set; }    // Timestamp indicating when the mood entry was created
    }
}
