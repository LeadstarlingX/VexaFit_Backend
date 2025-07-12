using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Exercise
{
    public class UpdateExerciseDTO : BaseDTO<int>
    {
        [Required]
        public int ExerciseCategoryId { get; set; }
        [Required]
        public int PositionCategoryId { get; set; }
        public int ImageId { get; set; }
        public int VideoId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
