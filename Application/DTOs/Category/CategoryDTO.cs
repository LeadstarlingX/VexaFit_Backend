using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.DTOs.Exercise;
using Application.DTOs.ExerciseCategory;
using Domain.Entities.AppEntities;
using Domain.Entities.Common;
using Domain.Enum;

namespace Application.DTOs.Category
{
    public class CategoryDTO : BaseDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public CategoryTypeEnum Type { get; set; }
    }
}
