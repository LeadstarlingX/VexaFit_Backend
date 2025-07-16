using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Domain.Enum;

namespace Application.DTOs.Category
{
    public class UpdateCategoryDTO : BaseDTO<int>
    {
        public string? Name { get; set; } = string.Empty;
        public CategoryTypeEnum? Type { get; set; }

    }
}
