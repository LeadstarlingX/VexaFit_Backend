using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Workout
{
    public class AddtoWorkoutDTO
    {
        public int workoutId { get; set; }
        public int exerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int? WeightKg { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
