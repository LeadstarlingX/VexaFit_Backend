using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.MotivationSentence;
using Application.IAppServices.MotivationSentece;

namespace Infrastructure.AppServices.MotivationSentnce
{
    public class MotivationSentenceService : IMotivationSentenceService
    {
        public Task<MotivationSentenceDTO> CreateAsync(CreateMotivationSentenceDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MotivationSentenceDTO>> CreateBulkAsync(IEnumerable<CreateMotivationSentenceDTO> dtos)
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

        public Task<IEnumerable<MotivationSentenceDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MotivationSentenceDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MotivationSentenceDTO> UpdateAsync(UpdateMotivationSentenceDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MotivationSentenceDTO>> UpdateBulkAsync(IEnumerable<UpdateMotivationSentenceDTO> dto)
        {
            throw new NotImplementedException();
        }
    }
}
