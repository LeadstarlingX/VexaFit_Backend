﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Workout
{
    public class GetWorkoutDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public WorkoutEnum Discriminator { get; set; } = WorkoutEnum.All;
        public string? UserId { get; set; }
    }
}
