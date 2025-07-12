using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Meal;

namespace Application.DTOs.Food
{
    public class CreateFoodDTO
    {
        [Required]
        [StringLength(10, ErrorMessage = "Food Name should be 10 characters minimum")]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int Calories { get; set; }
        [Required]
        public decimal Protein { get; set; }
        [Required]
        public decimal Carbs { get; set; }
        [Required]
        public decimal Fat { get; set; }

    }
}
