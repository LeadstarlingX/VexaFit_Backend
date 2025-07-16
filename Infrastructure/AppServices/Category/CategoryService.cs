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

namespace Infrastructure.AppServices.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppRepository<Domain.Entities.AppEntities.Category> _categoryReopsitory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDTO>> CreateBulkAsync(IEnumerable<CreateCategoryDTO> dtos)
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

        public Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDTO> UpdateAsync(UpdateCategoryDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDTO>> UpdateBulkAsync(IEnumerable<UpdateCategoryDTO> dto)
        {
            throw new NotImplementedException();
        }
    }
}
