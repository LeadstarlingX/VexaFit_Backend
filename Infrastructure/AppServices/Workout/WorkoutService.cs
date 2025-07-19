using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Exercise;
using Application.DTOs.Workout;
using Application.IAppServices.Workout;
using Application.IRepository;
using AutoMapper;
using Domain.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;
using WorkoutEntity = Domain.Entities.AppEntities.Workout;

namespace Infrastructure.AppServices.Workout
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IAppRepository<WorkoutEntity> _workoutRepository;
        private readonly IMapper _mapper;

        public WorkoutService(IAppRepository<WorkoutEntity> workoutRepository, IMapper mapper)
        {
            _workoutRepository = workoutRepository;
            _mapper = mapper;
        }


        public async Task<WorkoutDTO> GetByIdAsync(int id)
        {
            var query = _workoutRepository.FindWithComplexIncludes(x => x.Id == id,
                x => x.Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Images)
                .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Videos)
                .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise)
                .ThenInclude(x => x.ExerciseCategories).ThenInclude(x => x.Category));

            var entity = (await query.ToListAsync()).FirstOrDefault();
            return _mapper.Map<WorkoutDTO>(entity);

        }

        public async Task<IEnumerable<WorkoutDTO>> GetAllAsync(GetWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }



        public async Task<WorkoutDTO> CreateAsync(CreateWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WorkoutDTO>> CreateBulkAsync(IEnumerable<CreateWorkoutDTO> dtos)
        {
            throw new NotImplementedException();
        }



        public async Task<WorkoutDTO> UpdateAsync(UpdateWorkoutDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WorkoutDTO>> UpdateBulkAsync(IEnumerable<UpdateWorkoutDTO> dto)
        {
            throw new NotImplementedException();
        }



        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
