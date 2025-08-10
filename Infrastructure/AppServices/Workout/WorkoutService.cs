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
using System.Linq;
using Application.IUnitOfWork;

namespace Infrastructure.AppServices.Workout
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkoutService(IMapper mapper,IHttpContextAccessor httpContextAccessor,
            IAppUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<WorkoutDTO> GetByIdAsync(int id)
        {
            var query = GetBaseWorkoutQueryWithIncludes();

            query = ApplySecurityFilter(query);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
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
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();

            var entity = _mapper.Map<CustomWorkout>(dto);
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ((CustomWorkout)entity).UserId = userId!;
            await workoutRepository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WorkoutDTO>(entity);
        }

        public async Task<IEnumerable<WorkoutDTO>> CreateBulkAsync(IEnumerable<CreateWorkoutDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<WorkoutEntity>>(dtos);
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();
            await workoutRepository.BulkInsertAsync(entities);

            return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
        }



        public async Task<WorkoutDTO> UpdateAsync(UpdateWorkoutDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();

            var entity = (await workoutRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
            if (entity is null)
                throw new KeyNotFoundException("Workout not found");

            if (!isAdmin)
            {
                if (entity is PredefinedWorkout)
                    throw new UnauthorizedAccessException("Unauthorized: Predefined workouts cannot be modified.");

                if (entity is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new UnauthorizedAccessException("Unauthorized: This workout does not belong to you.");
            }

            _mapper.Map(dto, entity);

            await workoutRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WorkoutDTO>(entity);
        }
        

        public async Task<IEnumerable<WorkoutDTO>> UpdateBulkAsync(IEnumerable<UpdateWorkoutDTO> dto)
        {
            var entities = _mapper.Map<IEnumerable<WorkoutEntity>>(dto);
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();
            await workoutRepository.BulkUpdateAsync(entities);
            return _mapper.Map<IEnumerable<WorkoutDTO>>(entities);
        }



        public async Task DeleteAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();

            var entity = (await workoutRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity is null)
            {
                throw new KeyNotFoundException("Workout not found");
            }
            if (!isAdmin)
            {
                if (entity is PredefinedWorkout)
                    throw new UnauthorizedAccessException("Predefined workouts cannot be deleted.");

                if (entity is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new Exception("This workout doesn't belong to you");
            }
            await workoutRepository.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            if (ids is null || !ids.Any())
            {
                return;
            }
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();
            await workoutRepository.BulkRemoveAsync(ids);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task AddToWorkout(AddtoWorkoutDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();
            var exerciseRepository = _unitOfWork.Repository<ExerciseEntity>();
            var workoutExerciseRepository = _unitOfWork.Repository<WorkoutExercise>();

            var customWorkout = (await workoutRepository.FindAsync(x => x.Id == dto.workoutId)).FirstOrDefault();
            if (customWorkout is null)
                throw new Exception("Workout wasn't found");

            var exercise = (await exerciseRepository.FindAsync(x => x.Id == dto.exerciseId)).FirstOrDefault();
            if (exercise is null)
                throw new Exception("Exercise wasn't found");

            if (!isAdmin)
            {
                if (((CustomWorkout)customWorkout).UserId != userId)
                    throw new Exception("This workout doens't belong to you");
            }

            var entity = (await workoutExerciseRepository.FindAsync(x => (x.WorkoutId == dto.workoutId
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

            await workoutExerciseRepository.InsertAsync(newWorkoutExercise);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFromWorkout(DeleteFromWorkoutDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            var workoutExerciseRepository = _unitOfWork.Repository<WorkoutExercise>();

            var entityToDelete = await workoutExerciseRepository.GetAll()
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == dto.Id);

            if (entityToDelete is null)
                throw new KeyNotFoundException("The exercise link was not found in this workout.");

            if (!isAdmin)
            {
                if (entityToDelete.Workout is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new UnauthorizedAccessException("This workout does not belong to you.");
            }

            await workoutExerciseRepository.RemoveAsync(entityToDelete);
            await _unitOfWork.SaveChangesAsync();
        }



        public async Task UpdateExerciseInWorkout(UpdateWorkoutExerciseDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            var workoutExerciseRepository = _unitOfWork.Repository<WorkoutExercise>();

            var entity = await workoutExerciseRepository.GetAll()
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == dto.WorkoutExerciseId);

            if (entity is null)
                throw new KeyNotFoundException("Exercise entry not found in this workout.");

            if (!isAdmin)
            {
                if (entity.Workout is CustomWorkout customWorkout && customWorkout.UserId != userId)
                    throw new Exception("This workout doesn't belong to you.");
            }

            entity.Sets = dto.Sets;
            entity.Reps = dto.Reps;
            entity.WeightKg = dto.WeightKg;
            entity.DurationSeconds = dto.DurationSeconds;

            await workoutExerciseRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }


        private IQueryable<WorkoutEntity> GetBaseWorkoutQueryWithIncludes()
        {
            var workoutRepository = _unitOfWork.Repository<WorkoutEntity>();
            return workoutRepository.GetAll().AsSplitQuery()
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

            if (isAdmin)
            {
                return query;
            }

            
            query = query.Where(x => x is PredefinedWorkout || (x is CustomWorkout && ((CustomWorkout)x).UserId == userId));
            return query;

        }

        private IQueryable<WorkoutEntity> ApplyDtoFilters(IQueryable<WorkoutEntity> query, GetWorkoutDTO dto)
        {
            
            if (!string.IsNullOrEmpty(dto.Name))
            {
                query = query.Where(x => x.Name.Contains(dto.Name));
            }

            if (!string.IsNullOrEmpty(dto.Description))
            {
                query = query.Where(x => x.Description.Contains(dto.Description));
            }



            
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            if (isAdmin && !string.IsNullOrEmpty(dto.UserId))
            {
                query = query.OfType<CustomWorkout>().Where(cw => cw.UserId == dto.UserId);
            }


            
            switch (dto.Discriminator)
            {
                case WorkoutEnum.Custom:
                    query = query.OfType<CustomWorkout>();
                    break;
                case WorkoutEnum.Predefined:
                    query = query.OfType<PredefinedWorkout>();
                    break;
            }

            return query;
        }
    }
}
