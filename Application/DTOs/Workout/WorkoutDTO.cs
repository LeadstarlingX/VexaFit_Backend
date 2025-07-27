using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.DTOs.Exercise;
using Application.DTOs.Reminder;

namespace Application.DTOs.Workout
{
    public class WorkoutDTO : BaseDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


        public string? UserId { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }

        public ICollection<WorkoutExerciseDTO>? WorkoutExercises { get; set; }
        public ReminderDTO? Reminder { get; set; }
    }
}
