using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Exercise;
using Application.DTOs.Reminder;
using Application.DTOs.Workout;
using AutoMapper;
using Domain.Entities.AppEntities;

namespace Application.Mapping.WorkoutProfile
{
    public class WorkoutProfile : Profile
    {
        public WorkoutProfile() 
        {
            CreateMap<Workout, WorkoutDTO>()
                .Include<CustomWorkout, WorkoutDTO>()
                .Include<PredefinedWorkout, WorkoutDTO>()
                .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src =>
                    src.WorkoutExercises.Select(we => we.Exercise)))
                .ForMember(dest => dest.Reminder, opt => opt.MapFrom(src => src.WorkoutReminder));

            CreateMap<CreateWorkoutDTO, Workout>();
            CreateMap<CreateWorkoutDTO, CustomWorkout>();
            CreateMap<CustomWorkout, WorkoutDTO>();
            CreateMap<PredefinedWorkout, WorkoutDTO>();
            CreateMap<WorkoutReminder, ReminderDTO>();
            CreateMap<WorkoutReminderDate, ReminderDateDTO>();

        }
    }
}
