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
        public int UserId { get; set; }
        public DateTime TimeOfDay { get; set; }
        public string Days { get; set; }
        public bool IsActive { get; set; }
    }
}
