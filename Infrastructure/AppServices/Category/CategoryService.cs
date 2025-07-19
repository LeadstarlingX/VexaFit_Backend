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

namespace Infrastructure.AppServices.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppRepository<CategoryEntity> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IAppRepository<CategoryEntity> categoryReopsitory,
            UserManager<ApplicationUser> userManager, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryReopsitory;
            _mapper = mapper;
        }


        public async Task<CategoryDTO> GetByIdAsync(int id)
        {
            var entity = (await _categoryRepository.FindWithAllIncludeAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
                throw new Exception("Category not found");
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(GetCategoryDTO dto)
        {
            var query = _categoryRepository.GetAllWithAllInclude();
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
            var entity = _mapper.Map<CategoryEntity>(dto);
            await _categoryRepository.InsertAsync(entity);

            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> CreateBulkAsync(IEnumerable<CreateCategoryDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<CategoryEntity>>(dtos);
            await _categoryRepository.BulkInsertAsync(entities);

            return _mapper.Map<IEnumerable<CategoryDTO>>(entities);
        }



        public async Task<CategoryDTO> UpdateAsync(UpdateCategoryDTO dto)
        {
            var entity = (await _categoryRepository.FindAsync(x => x.Id == dto.Id)).FirstOrDefault();
            if(entity == null)
                throw new Exception("Category not found");


            entity = _mapper.Map<CategoryEntity>(dto);
            await _categoryRepository.UpdateAsync(entity);
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<IEnumerable<CategoryDTO>> UpdateBulkAsync(IEnumerable<UpdateCategoryDTO> dto)
        {
            var entities = _mapper.Map<IEnumerable<CategoryEntity>>(dto);
            await _categoryRepository.BulkUpdateAsync(entities);
            return _mapper.Map<IEnumerable<CategoryDTO>>(entities);
        }
        


        public async Task DeleteAsync(int id)
        {
            var entity = (await _categoryRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            await _categoryRepository.RemoveAsync(entity);
            return;
        }
       
        public async Task DeleteBulkAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return;
            }

            await _categoryRepository.BulkRemoveAsync(ids);

        }

    }
}
