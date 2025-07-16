using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Application.DTOs.Exercise
{
    public class CreateExerciseDTO
    {
        [Required]
        [StringLength(10, ErrorMessage = "Exercise Name should be 10 characters minimum")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(10, ErrorMessage = "Description should be 10 characters minimum")]
        public string Description { get; set; } = string.Empty;
    }
}
