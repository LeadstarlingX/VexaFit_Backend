using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

namespace Application.DTOs.Exercise
{
    public class CreateExerciseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int WorkoutId { get; set; }
        public IEnumerable<ImageDTO> ImageFiles { get; set; } = [];
        public IEnumerable<VideoDTO> VideoFiles { get; set; } = [];

    }
}
