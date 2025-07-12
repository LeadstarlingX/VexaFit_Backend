using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Achievement
{
    public class CreateAchievementDTO
    {
        [Required]
        [StringLength(10, ErrorMessage = "Name of Achievement should be 10 characters minimum")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(10, ErrorMessage = "Description should be 10 characters minimum")]
        public string Description = string.Empty;
        [Required]
        public string Criteria = string.Empty;
    }
}
