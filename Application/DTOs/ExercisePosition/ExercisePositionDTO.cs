using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Common;
using Application.DTOs.Exercise;

namespace Application.DTOs.ExercisePosition
{
    public class ExercisePositionDTO : BaseDTO<int>
    {
        public int ExerciseId { get; set; }
        public int CategoryId { get; set; }
        public ExerciseDTO? Exercise { get; set; }
        public CategoryDTO? Category { get; set; }
    }
}
