using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Reminder;
using Application.IAppServices.Reminder;

namespace Infrastructure.AppServices.Reminder
{
    public class ReminderService : IReminderService
    {
        public Task<ReminderDTO> CreateAsync(CreateReminderDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReminderDTO>> CreateBulkAsync(IEnumerable<CreateReminderDTO> dtos)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReminderDTO>> GetAllAsync(GetReminderDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ReminderDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ReminderDTO> UpdateAsync(UpdateReminderDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReminderDTO>> UpdateBulkAsync(IEnumerable<UpdateReminderDTO> dto)
        {
            throw new NotImplementedException();
        }
    }
}
