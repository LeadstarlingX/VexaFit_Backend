using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.IAppServices.Category;
using Application.IAppServices.Common;
using Application.IRepository;
using AutoMapper;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;


using CategoryEntity = Domain.Entities.AppEntities.Category;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Application.IUnitOfWork;

namespace Infrastructure.AppServices.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IAppUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<CategoryDTO> GetByIdAsync(int id)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entity = (await categoryRepository.FindWithAllIncludeAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
                throw new Exception("Category not found");
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(GetCategoryDTO dto)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var query = categoryRepository.GetAllWithAllInclude();
            query = query.Include(x => x.ExerciseCategories).ThenInclude(x => x.Exercise);
            if(dto.Name != null)
                query = query.Where(x => x.Name.Contains(dto.Name));
            if (dto.Type != null)
                query = query.Where(x => x.Type == dto.Type);

            var entites = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(entites);
        }



        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entity = _mapper.Map<CategoryEntity>(dto);
            await categoryRepository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> CreateBulkAsync(IEnumerable<CreateCategoryDTO> dtos)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entities = _mapper.Map<IEnumerable<CategoryEntity>>(dtos);
            await categoryRepository.BulkInsertAsync(entities);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(entities);
        }



        public async Task<CategoryDTO> UpdateAsync(UpdateCategoryDTO dto)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entity = (await categoryRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
            if(entity == null)
                throw new Exception("Category not found");


            _mapper.Map(dto, entity);
            await categoryRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> UpdateBulkAsync(IEnumerable<UpdateCategoryDTO> dto)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entities = _mapper.Map<IEnumerable<CategoryEntity>>(dto);
            await categoryRepository.BulkUpdateAsync(entities);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(entities);
        }
        


        public async Task DeleteAsync(int id)
        {
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            var entity = (await categoryRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            await categoryRepository.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return;
        }
       
        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return;
            }
            var categoryRepository = _unitOfWork.Repository<CategoryEntity>();
            await categoryRepository.BulkRemoveAsync(ids);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
