using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Reminder;
using Application.IAppServices.Common;

namespace Application.IAppServices.Reminder
{
    public interface IReminderService : IService<ReminderDTO, CreateReminderDTO,
        UpdateReminderDTO, GetReminderDTO>
    {
    }
}
