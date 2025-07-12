using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reminder
{
    public class CreateReminderDTO
    {
        public int UserId { get; set; }
        [Required]
        public DateTime TimeOfDay { get; set; }
        [Required]
        public string Days { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
