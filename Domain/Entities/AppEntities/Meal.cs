using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Domain.Entities.AppEntities
{
    public class Meal : BaseEntity
    {
        public int MealId { get; set; }
        public int UserMealId { get; set; }
        public int FoodId { get; set; }
        public decimal Quantity { get; set; }

        public UserMeal UserMeal { get; set; } = null!;
        public Food Food { get; set; } = null!;
    }
}
