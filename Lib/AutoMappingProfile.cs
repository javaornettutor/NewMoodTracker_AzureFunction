using AutoMapper;
using MoodTrackerAPI_2.Dto;

using NewMoodTracker_AzureFunction.Models;

/*
 * Author: William Wang
 * Email: wangwilliam2010@gmail.com
 * 
 * This class defines a mapping profile for AutoMapper, enabling the transformation
 * of data transfer objects (DTOs) to business objects (BOs) and vice versa.
 * This helps in the separation of concerns by decoupling the data models used in 
 * the API from those used in the database layer.
 */

namespace MoodTrackerAPI_2.Lib
{
    public class AutoMappingProfile : Profile
    {
        // Constructor initializes mapping configurations between DTO and BO classes.
        public AutoMappingProfile()
        {
            // Maps UserMoodDto to UserMood, allowing for data transformation between layers.
            CreateMap<UserMoodDto, UserMood>();

            // Additional mappings can be configured here as needed.
        }
    }
}
