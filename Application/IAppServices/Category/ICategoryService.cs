using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.IAppServices.Common;

namespace Application.IAppServices.Category
{
    public interface ICategoryService : IService<CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO>
    {

    }
}
