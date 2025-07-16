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
        public int ExerciseCategoryId { get; set; }
        public int PositionCategoryId { get; set; }
        public int ImageId { get; set; }
        public int VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<WorkoutExerciseDTO>? WorkoutExercises { get; set; } = []!;
        public ImageDTO? Image { get; set; } = null!;
        public VideoDTO? Video { get; set; } = null!;
        public CategoryDTO? Category { get; set; } = null!;
    }
}
