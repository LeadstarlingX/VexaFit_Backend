using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Workout
{
    public class UpdateWorkoutExerciseDTO
    {
        public int WorkoutExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int? WeightKg { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
