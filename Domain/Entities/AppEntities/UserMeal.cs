using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class UserMeal : BaseEntity
    {
        public int UserMealId { get; set; }
        public Guid UserId { get; set; }
        public string MealType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public ICollection<Meal>? Meals { get; set; } = [];
    }
}
