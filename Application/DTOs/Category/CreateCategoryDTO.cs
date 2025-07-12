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
        [Required]
        [StringLength(10, ErrorMessage = "Category name should be 10 character minimum")]
        public string Name { get; set; } = string.Empty;
        [Required]
        public CategoryTypeEnum TypeEnum { get; set; }

    }
}
