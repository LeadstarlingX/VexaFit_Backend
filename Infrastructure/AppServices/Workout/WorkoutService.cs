using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
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

            var entity = (await query.AsNoTracking().ToListAsync()).FirstOrDefault();
            return _mapper.Map<WorkoutDTO>(entity);

        }

        public async Task<IEnumerable<WorkoutDTO>> GetAllAsync(GetWorkoutDTO dto)
        {
            if (((int)dto.Discreminator) == 0)
            {
                var query = _workoutRepository.GetAllWithAllInclude();
                query = query.Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Images)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Videos)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise);

                if (dto.Description != null)
                    query = query.Where(x => x.Description.Contains(dto.Description));
                if (dto.Name != null)
                    query = query.Where(x => x.Name.Contains(dto.Name));

                var entities = await query.ToListAsync();
                return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);

            }
            else if (((int)dto.Discreminator) == 1)
            {
                var query = _workoutRepository.GetAllWithAllInclude().OfType<CustomWorkout>();
                if (dto.UserId != null)
                    query = query.Where(x => x.UserId == dto.UserId);
                query = query.Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Images)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Videos)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise);

                if (dto.Description != null)
                    query = query.Where(x => x.Description.Contains(dto.Description));
                if (dto.Name != null)
                    query = query.Where(x => x.Name.Contains(dto.Name));

                var entities = await query.ToListAsync();
                return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
            }
            else 
            {
                var query = _workoutRepository.GetAllWithAllInclude().OfType<PredefinedWorkout>();
                query = query.Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Images)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise).ThenInclude(x => x.Videos)
                    .Include(x => x.WorkoutExercises).ThenInclude(x => x.Exercise);

                if (dto.Description != null)
                    query = query.Where(x => x.Description.Contains(dto.Description));
                if (dto.Name != null)
                    query = query.Where(x => x.Name.Contains(dto.Name));

                var entities = await query.ToListAsync();
                return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
            };

        }



        public async Task<WorkoutDTO> CreateAsync(CreateWorkoutDTO dto)
        {
            var entity = _mapper.Map<WorkoutEntity>(dto);
            await _workoutRepository.InsertAsync(entity);
            return _mapper.Map<WorkoutDTO>(entity);
        }

        public async Task<IEnumerable<WorkoutDTO>> CreateBulkAsync(IEnumerable<CreateWorkoutDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<WorkoutEntity>>(dtos);
            await _workoutRepository.BulkInsertAsync(entities);

            return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
        }



        public async Task<WorkoutDTO> UpdateAsync(UpdateWorkoutDTO dto)
        {
            var entity = _mapper.Map<WorkoutEntity>(dto);
            await _workoutRepository.UpdateAsync(entity);
            return _mapper.Map<WorkoutDTO>(entity);
        }

        public async Task<IEnumerable<WorkoutDTO>> UpdateBulkAsync(IEnumerable<UpdateWorkoutDTO> dto)
        {
            var entities = _mapper.Map<IEnumerable<WorkoutEntity>>(dto);
            await _workoutRepository.BulkUpdateAsync(entities);
            return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
        }



        public async Task DeleteAsync(int id)
        {
            var entity = (await _workoutRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException("Workout not found");
            }
            await _workoutRepository.RemoveAsync(entity);
            return;
        }

        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return;
            }

            await _workoutRepository.BulkRemoveAsync(ids);
        }
    }
}
