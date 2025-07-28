using System.Security.Claims;
using Application.DTOs.Workout;
using Application.IAppServices.Workout;
using Application.IRepository;
using AutoMapper;
using Domain.Entities.AppEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkoutEntity = Domain.Entities.AppEntities.Workout;
using ExerciseEntity = Domain.Entities.AppEntities.Exercise;

namespace Infrastructure.AppServices.Workout
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IAppRepository<WorkoutEntity> _workoutRepository;
        private readonly IAppRepository<WorkoutExercise> _workoutExerciseRepository;
        private readonly IAppRepository<ExerciseEntity> _exerciseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WorkoutService(IAppRepository<WorkoutEntity> workoutRepository,
            IAppRepository<WorkoutExercise> workoutExerciseRepository,
            IAppRepository<ExerciseEntity> exerciseRepository, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _workoutRepository = workoutRepository;
            _workoutExerciseRepository = workoutExerciseRepository;
            _exerciseRepository = exerciseRepository;
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
            var entity = _mapper.Map<CustomWorkout>(dto);
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ((CustomWorkout)entity).UserId = userId!;
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

            var entity = (await _workoutRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
            if (entity == null)
                throw new KeyNotFoundException("Workout not found");

            if (!isAdmin)
            {
                if (entity is PredefinedWorkout)
                    throw new UnauthorizedAccessException("Unauthorized: Predefined workouts cannot be modified.");

                if (entity is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new UnauthorizedAccessException("Unauthorized: This workout does not belong to you.");
            }

            _mapper.Map(dto, entity);

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
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;


            var entity = (await _workoutRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException("Workout not found");
            }
            if (!isAdmin)
            {
                if (((CustomWorkout)entity).UserId != userId)
                    throw new Exception("This workout doesn't belong to you");
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


        public async Task AddToWorkout(AddtoWorkoutDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            
            var customWorkout = (await _workoutRepository.FindAsync(x => x.Id == dto.workoutId)).FirstOrDefault();
            if (customWorkout == null)
                throw new Exception("Workout wasn't found");

            var exercise = (await _exerciseRepository.FindAsync(x => x.Id == dto.exerciseId)).FirstOrDefault();
            if (exercise == null)
                throw new Exception("Exercise wasn't found");

            if (!isAdmin)
            {
                if (((CustomWorkout)customWorkout).UserId != userId)
                    throw new Exception("This workout doens't belong to you");
            }

            var entity = (await _workoutExerciseRepository.FindAsync(x => (x.WorkoutId == dto.workoutId
            && x.ExerciseId == dto.exerciseId))).FirstOrDefault();
            if (entity is not null)
            {
                throw new Exception("This exercise already belong to this workout");
            }

            var newWorkoutExercise = new WorkoutExercise
            {
                WorkoutId = dto.workoutId,
                ExerciseId = dto.exerciseId,
                Sets = dto.Sets,
                Reps = dto.Reps,
                WeightKg = dto.WeightKg, 
                DurationSeconds = dto.DurationSeconds 
            };

            await _workoutExerciseRepository.InsertAsync(newWorkoutExercise);
        }

        public async Task DeleteFromWorkout(DeleteFromWorkoutDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

           
            var entityToDelete = await _workoutExerciseRepository.GetAll()
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == dto.Id);

            if (entityToDelete == null)
                throw new KeyNotFoundException("The exercise link was not found in this workout.");

            if (!isAdmin)
            {
                if (entityToDelete.Workout is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new UnauthorizedAccessException("This workout does not belong to you.");
            }

            await _workoutExerciseRepository.RemoveAsync(entityToDelete);
        }



        public async Task UpdateExerciseInWorkout(UpdateWorkoutExerciseDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = await _workoutExerciseRepository.GetAll()
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == dto.WorkoutExerciseId);

            if (entity == null)
                throw new KeyNotFoundException("Exercise entry not found in this workout.");

            if (entity.Workout is CustomWorkout customWorkout && customWorkout.UserId != userId)
                throw new Exception("This workout doesn't belong to you.");

            entity.Sets = dto.Sets;
            entity.Reps = dto.Reps;
            entity.WeightKg = dto.WeightKg;
            entity.DurationSeconds = dto.DurationSeconds;

            await _workoutExerciseRepository.UpdateAsync(entity);
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
