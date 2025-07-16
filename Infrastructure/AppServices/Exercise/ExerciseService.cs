using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Exercise;
using Application.IAppServices.Exercise;

namespace Infrastructure.AppServices.Exercise
{
    public class ExerciseService : IExerciseService
    {
        public Task<ExerciseDTO> CreateAsync(CreateExerciseDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ExerciseDTO>> CreateBulkAsync(IEnumerable<CreateExerciseDTO> dtos)
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

        public Task<IEnumerable<ExerciseDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExerciseDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ExerciseDTO> UpdateAsync(UpdateExerciseDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ExerciseDTO>> UpdateBulkAsync(IEnumerable<UpdateExerciseDTO> dto)
        {
            throw new NotImplementedException();
        }
    }
}
