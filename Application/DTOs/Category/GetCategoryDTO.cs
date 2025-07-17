using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs.Category
{
    public class GetCategoryDTO
    {
        public string? Name { get; set; }
        public CategoryTypeEnum? Type { get; set; }
    }
}
