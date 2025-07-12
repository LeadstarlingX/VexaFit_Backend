using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Domain.Entities.Common;

namespace Application.DTOs.Category
{
    public class CategoryDTO : BaseDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Type {  get; set; } = string.Empty;
    }
}
