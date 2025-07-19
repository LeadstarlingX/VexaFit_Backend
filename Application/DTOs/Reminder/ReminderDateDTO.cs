using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Reminder
{
    public class ReminderDateDTO : BaseDTO<int>
    {
        public DateTime DateWithTime { get; set; }
    }
}
