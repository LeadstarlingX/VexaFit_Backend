﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Domain.Entities.AppEntities
{
    public class WorkoutExercise : BaseEntity
    {
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }


        public virtual Workout Workout { get; set; } = null!;
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
