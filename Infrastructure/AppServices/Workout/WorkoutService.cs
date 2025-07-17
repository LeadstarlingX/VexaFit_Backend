using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Workout;
using Application.IAppServices.Workout;

namespace Infrastructure.AppServices.Workout
{
    public class WorkoutService : IWorkoutService
    {
        public Task<WorkoutDTO> CreateAsync(CreateWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkoutDTO>> CreateBulkAsync(IEnumerable<CreateWorkoutDTO> dtos)
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

        public Task<IEnumerable<WorkoutDTO>> GetAllAsync(GetWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<WorkoutDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<WorkoutDTO> UpdateAsync(UpdateWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkoutDTO>> UpdateBulkAsync(IEnumerable<UpdateWorkoutDTO> dto)
        {
            throw new NotImplementedException();
        }
    }
}
