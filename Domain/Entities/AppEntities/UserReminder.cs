using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class UserReminder : BaseEntity
    {
        public int ReminderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime TimeOfDay { get; set; }
        public string Days { get; set; }
        public bool IsActive { get; set; }

    }
}
