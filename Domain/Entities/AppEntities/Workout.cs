using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class Workout : BaseEntity
    {
        public int WorkoutId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<UserHistory> UserHistories { get; set; } = [];
        public ICollection<WorkoutExercise>? WorkoutExercises { get; set; } = [];
    }
}
