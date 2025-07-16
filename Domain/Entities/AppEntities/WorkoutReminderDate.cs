using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class WorkoutReminderDate : BaseEntity
    {
        public int WorkoutReminderDateId { get; set; }
        public int WorkoutReminderId { get; set; }


        public DateTime DateWithTime { get; set; }
    }
}
