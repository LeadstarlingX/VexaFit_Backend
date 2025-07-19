using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Exercise;
using Application.DTOs.Workout;
using Application.IAppServices.Workout;
using Application.IRepository;
using AutoMapper;
using Domain.Entities.AppEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkoutEntity = Domain.Entities.AppEntities.Workout;

namespace Infrastructure.AppServices.Workout
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IAppRepository<WorkoutEntity> _workoutRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WorkoutService(IAppRepository<WorkoutEntity> workoutRepository, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _workoutRepository = workoutRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }


        public async Task<WorkoutDTO> GetByIdAsync(int id)
        {
            var query = GetBaseWorkoutQueryWithIncludes();

            query = ApplySecurityFilter(query);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new Exception("Workout not found");
            return _mapper.Map<WorkoutDTO>(entity);
        }

        public async Task<IEnumerable<WorkoutDTO>> GetAllAsync(GetWorkoutDTO dto)
        { 
            var query = GetBaseWorkoutQueryWithIncludes();

            query = ApplySecurityFilter(query);

            query = ApplyDtoFilters(query, dto);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
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
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            if (isAdmin)
            {
                var entity = (await _workoutRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
                if (entity == null)
                    throw new Exception("Workout not found");
                entity = _mapper.Map<WorkoutEntity>(dto);
                await _workoutRepository.UpdateAsync(entity);
                return _mapper.Map<WorkoutDTO>(entity);
            }
            else
            {
                var entity = (await _workoutRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
                if (entity == null)
                    throw new Exception("Workout not found");

                if (entity is PredefinedWorkout)
                    throw new Exception("Unauthorized data, this workout is predefined");

                if ((entity as CustomWorkout)!.UserId != userId)
                    throw new Exception("Unauthorized data, this workout doesn't belong to this user");

                await _workoutRepository.UpdateAsync(entity);
                return _mapper.Map<WorkoutDTO>(entity);
            }
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



        private IQueryable<WorkoutEntity> GetBaseWorkoutQueryWithIncludes()
        {
            return _workoutRepository.GetAll()
                .Include(x => x.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                        .ThenInclude(e => e.Images)
                .Include(x => x.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                        .ThenInclude(e => e.Videos)
                .Include(x => x.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                        .ThenInclude(e => e.ExerciseCategories)
                            .ThenInclude(ec => ec.Category)
                .AsNoTracking();
        }

        private IQueryable<WorkoutEntity> ApplySecurityFilter(IQueryable<WorkoutEntity> query)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            if (!isAdmin)
            {
                // A user can see a workout if it's Predefined OR it's a CustomWorkout they own.
                query = query.Where(x => x is PredefinedWorkout || (x is CustomWorkout && ((CustomWorkout)x).UserId == userId));
            }

            return query;
        }

        private IQueryable<WorkoutEntity> ApplyDtoFilters(IQueryable<WorkoutEntity> query, GetWorkoutDTO dto)
        {
            // Filter by specific type (Custom or Predefined)
            switch (dto.Discriminator)
            {
                case WorkoutEnum.Custom:
                    query = query.OfType<CustomWorkout>();
                    break;
                case WorkoutEnum.Predefined:
                    query = query.OfType<PredefinedWorkout>();
                    break;
            }

            // Filter by text properties
            if (!string.IsNullOrEmpty(dto.Name))
            {
                query = query.Where(x => x.Name.Contains(dto.Name));
            }

            if (!string.IsNullOrEmpty(dto.Description))
            {
                query = query.Where(x => x.Description.Contains(dto.Description));
            }

            // Special filter for Admins looking for workouts by a specific user
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            if (isAdmin && !string.IsNullOrEmpty(dto.UserId))
            {
                // We must first filter to CustomWorkout to access the UserId property
                query = query.OfType<CustomWorkout>().Where(cw => cw.UserId == dto.UserId);
            }

            return query;
        }
    }
}
