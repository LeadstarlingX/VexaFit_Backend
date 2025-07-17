using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs.Category
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public CategoryTypeEnum TypeEnum { get; set; }

    }
}
