﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.DTOs.Exercise;

namespace Application.DTOs.Workout
{
    public class WorkoutExerciseDTO : BaseDTO<int>
    {
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int? WeightKg { get; set; }
        public int? DurationSeconds { get; set; }

        public ExerciseDTO? Exercise { get; set; }
    }
}
