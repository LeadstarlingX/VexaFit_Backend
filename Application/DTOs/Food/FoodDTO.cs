using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.DTOs.Meal;

namespace Application.DTOs.Food
{
    public class FoodDTO : BaseDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public int Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }

        public ICollection<MealDTO>? Meals { get; set; } = [];
    }
}
