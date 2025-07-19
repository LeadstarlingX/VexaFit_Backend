using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Reminder
{
    public class ReminderDTO : BaseDTO<int>
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsReminded { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ReminderDateDTO> WorkoutReminderDates { get; set; } = [];
    }
}
