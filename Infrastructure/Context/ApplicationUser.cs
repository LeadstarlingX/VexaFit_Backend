using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.AppEntities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Context
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserGenderEnum GenderEnum { get; set; } = UserGenderEnum.Male;
        public UserGoalEnum GoalEnum { get; set; } = UserGoalEnum.WeightGain;
        public int Age { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int Weight { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
    }
}
