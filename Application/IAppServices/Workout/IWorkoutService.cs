﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.DTOs.Workout;
using Application.IAppServices.Common;

namespace Application.IAppServices.Workout
{
    public interface IWorkoutService : IService<WorkoutDTO, CreateWorkoutDTO,
        UpdateWorkoutDTO, GetWorkoutDTO>
    {
        public Task AddToWorkout(AddtoWorkoutDTO dto);
        public Task DeleteFromWorkout(DeleteFromWorkoutDTO dto);
        public Task UpdateExerciseInWorkout(UpdateWorkoutExerciseDTO dto);
    }
}
