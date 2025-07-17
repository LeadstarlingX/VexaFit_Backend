using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class WorkoutReminder : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int WorkoutId { get; set; }
        

        public bool IsActive { get; set; }
        public bool IsReminded { get; set; }


        public virtual Workout Workout { get; set; } = null!;
        public virtual ICollection<WorkoutReminderDate> WorkoutReminderDates { get; set; } = [];
    }
}
