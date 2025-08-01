﻿using System;
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
        public UserGenderEnum GenderEnum { get; set; } = UserGenderEnum.Male;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public virtual ICollection<WorkoutReminder> WorkoutReminders { get; set; } = [];
        public virtual ICollection<CustomWorkout> CustomWorkouts { get; set; } = [];
        
    }
}
