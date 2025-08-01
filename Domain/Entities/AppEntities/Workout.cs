﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class Workout : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];

        public virtual WorkoutReminder? WorkoutReminder { get; set; }
    }
}
