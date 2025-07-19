using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Exercise;
using Application.IAppServices.Exercise;
using Application.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;

using ExerciseEntity = Domain.Entities.AppEntities.Exercise;
using ExerciseCategoryEntity = Domain.Entities.AppEntities.ExerciseCategory;
using WorkoutExerciseEntity = Domain.Entities.AppEntities.WorkoutExercise;
using ImageEntity = Domain.Entities.AppEntities.Image;
using VideoEntity = Domain.Entities.AppEntities.Video;
using Domain.Entities.AppEntities;
using Application.DTOs.Category;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.AppServices.Exercise
{
    public class ExerciseService : IExerciseService
    {
        private readonly IAppRepository<ExerciseEntity> _exerciseRepository;
        private readonly IAppRepository<ExerciseCategoryEntity> _exerciseCategoryRepository;
        private readonly IAppRepository<ImageEntity> _imageRepository;
        private readonly IAppRepository<VideoEntity> _videoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ExerciseService(IAppRepository<ExerciseEntity> exerciseRepository,
            IAppRepository<ExerciseCategoryEntity> exerciseCategoryRepository,
            IAppRepository<WorkoutExerciseEntity> workoutExerciseRepository,
            IAppRepository<ImageEntity> imageRepository, IAppRepository<VideoEntity> videoRepository,
            IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _exerciseRepository = exerciseRepository;
            _exerciseCategoryRepository = exerciseCategoryRepository;
            _imageRepository = imageRepository;
            _videoRepository = videoRepository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }
        
        
        public async Task<ExerciseDTO> GetByIdAsync(int id)
        {
            var query =  _exerciseRepository.FindWithComplexIncludes(x => x.Id == id,
                x => x.Include(x => x.Images)
                .Include(x => x.Videos)
                .Include(x => x.ExerciseCategories).ThenInclude(x => x.Category));

            var entity = (await query.ToListAsync()).FirstOrDefault();
            return _mapper.Map<ExerciseDTO>(entity);
        }

        public async Task<IEnumerable<ExerciseDTO>> GetAllAsync(GetExerciseDTO dto)
        {
            var query =  _exerciseRepository.GetAllWithAllInclude();
            query = query.Include(x => x.Images)
                .Include(x => x.Videos)
                .Include(x => x.ExerciseCategories).ThenInclude(x => x.Category);

            if (dto.Description != null)
                query = query.Where(x => x.Description.Contains(dto.Description));
            if (dto.Name != null)
                query = query.Where(x => x.Name.Contains(dto.Name));

            var entites = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<ExerciseDTO>>(entites);
        }



        public async Task<ExerciseDTO> CreateAsync(CreateExerciseDTO dto)
        {
            var exerciseEntity = _mapper.Map<ExerciseEntity>(dto);
            await _exerciseRepository.InsertAsync(exerciseEntity);

            var exerciseCategoryEntity = new ExerciseCategoryEntity { ExerciseId = exerciseEntity.Id,
                CategoryId = dto.CategoryId };
            await _exerciseCategoryRepository.InsertAsync(exerciseCategoryEntity);


            if (dto.ImageFiles.Any())
            {
                foreach(var imageDTO in dto.ImageFiles)
                {
                    if (imageDTO.ImageFile == null) continue;
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageDTO.ImageFile.FileName);
                    string directoryPath = Path.Combine(wwwRootPath, "images");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string filePath = Path.Combine(directoryPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageDTO.ImageFile.CopyToAsync(fileStream);
                    }

                    var alternativeText = imageDTO.AlternativeText != null ?
                        imageDTO.AlternativeText : imageDTO.ImageFile.FileName;

                    var imageEntity = new ImageEntity { ExerciseId = exerciseEntity.Id,
                    AlternativeText = alternativeText, Url = fileName};
                    var result = await _imageRepository.InsertAsync(imageEntity);

                    if (result == null)
                    {
                        throw new InvalidOperationException($"The insertion of image {imageEntity.AlternativeText}");
                    }
                }
            }

            if (dto.VideoFiles.Any())
            {
                foreach (var videoDTO in dto.VideoFiles)
                {
                    if (videoDTO.VideoFile == null) continue;
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoFile.FileName);
                    string directoryPath = Path.Combine(wwwRootPath, "videos");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string filePath = Path.Combine(directoryPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await videoDTO.VideoFile.CopyToAsync(fileStream);
                    }

                    var description = videoDTO.Description != null ?
                        videoDTO.Description : videoDTO.VideoFile.FileName;

                    var videoEntity = new VideoEntity
                    {
                        ExerciseId = exerciseEntity.Id,
                        Description = description,
                        Url = fileName
                    };
                    var result = await _videoRepository.InsertAsync(videoEntity);

                    if (result == null)
                    {
                        throw new InvalidOperationException($"The insertion of video {videoEntity.Description}");
                    }
                }
            }

            
            return _mapper.Map<ExerciseDTO>(exerciseEntity);
        }

        public async Task<IEnumerable<ExerciseDTO>> CreateBulkAsync(IEnumerable<CreateExerciseDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return new List<ExerciseDTO>();
            }

            var exerciseEntities = _mapper.Map<List<ExerciseEntity>>(dtos);
            await _exerciseRepository.BulkInsertAsync(exerciseEntities);

            var exerciseCategoriesToCreate = new List<ExerciseCategoryEntity>();
            var workoutExercisesToCreate = new List<WorkoutExerciseEntity>();
            var imagesToCreate = new List<ImageEntity>();
            var videosToCreate = new List<VideoEntity>();

            for (int i = 0; i < dtos.Count(); i++)
            {
                var dto = dtos.ElementAt(i);
                var createdExercise = exerciseEntities[i];

                exerciseCategoriesToCreate.Add(new ExerciseCategoryEntity { ExerciseId = createdExercise.Id, CategoryId = dto.CategoryId });
               
                if (dto.ImageFiles != null)
                {
                    foreach (var imageDTO in dto.ImageFiles)
                    {
                        if (imageDTO.ImageFile == null) continue;

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageDTO.ImageFile.FileName);
                        string directoryPath = Path.Combine(wwwRootPath, "images");

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        string filePath = Path.Combine(directoryPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageDTO.ImageFile.CopyToAsync(fileStream);
                        }

                        var alternativeText = imageDTO.AlternativeText != null ?
                            imageDTO.AlternativeText : imageDTO.ImageFile.FileName;

                        imagesToCreate.Add(new ImageEntity
                        {
                            ExerciseId = createdExercise.Id,
                            AlternativeText = alternativeText,
                            Url = fileName
                        });
                    }
                }

                if (dto.VideoFiles != null)
                {
                    foreach (var videoDTO in dto.VideoFiles)
                    {
                        if (videoDTO.VideoFile == null) continue;

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoFile.FileName);
                        string directoryPath = Path.Combine(wwwRootPath, "videos");

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        string filePath = Path.Combine(directoryPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await videoDTO.VideoFile.CopyToAsync(fileStream);
                        }

                        var description = videoDTO.Description != null ?
                            videoDTO.Description : videoDTO.VideoFile.FileName;

                        videosToCreate.Add(new VideoEntity
                        {
                            ExerciseId = createdExercise.Id,
                            Description = description,
                            Url = fileName
                        });
                       
                    }
                }

            }
                if (exerciseCategoriesToCreate.Any()) await _exerciseCategoryRepository.BulkInsertAsync(exerciseCategoriesToCreate);
                if (imagesToCreate.Any()) await _imageRepository.BulkInsertAsync(imagesToCreate);
                if (videosToCreate.Any()) await _videoRepository.BulkInsertAsync(videosToCreate);

                return _mapper.Map<IEnumerable<ExerciseDTO>>(exerciseEntities);
        }



        public async Task<ExerciseDTO> UpdateAsync(UpdateExerciseDTO dto)
        {
            var entity = _mapper.Map<ExerciseEntity>(dto);
            await _exerciseRepository.UpdateAsync(entity);
            return _mapper.Map<ExerciseDTO>(entity);
        }

        public async Task<IEnumerable<ExerciseDTO>> UpdateBulkAsync(IEnumerable<UpdateExerciseDTO> dto)
        {
            var entities = _mapper.Map<IEnumerable<ExerciseEntity>>(dto);
            await _exerciseRepository.BulkUpdateAsync(entities);
            return _mapper.Map<IEnumerable<ExerciseDTO>>(entities);
        }



        public async Task DeleteAsync(int id)
        {
            var entity = (await _exerciseRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException("Exercise not found");
            }
            await _exerciseRepository.RemoveAsync(entity);
            return;
        }

        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return;
            }

            await _exerciseRepository.BulkRemoveAsync(ids);
        }

    }

}