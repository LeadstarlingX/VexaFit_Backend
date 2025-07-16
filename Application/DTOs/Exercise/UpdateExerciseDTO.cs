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
        public int ImageId { get; set; }
        public int VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
