using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Common;
using Domain.Entities.AppEntities;

namespace Application.DTOs.Exercise
{
    public class ExerciseDTO : BaseDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ImageDTO? Image { get; set; } = null!;
        public VideoDTO? Video { get; set; } = null!;
        public ICollection<CategoryDTO>? Categories { get; set; } = [];
    }
}
