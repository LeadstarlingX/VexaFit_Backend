using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class UserHistory : BaseEntity
    {
        public int HistoryId { get; set; }
        public Guid UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime DateComplted { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public int DailyStreak { get; set; }
        public Workout Workout { get; set; } = null!;

    }
}
