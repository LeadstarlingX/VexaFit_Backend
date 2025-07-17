using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.AppEntities;
using Infrastructure.Converters;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        #region Tables
        public override DbSet<ApplicationRole> Roles { get; set; }
        public override DbSet<ApplicationUser> Users { get; set; }
        internal DbSet<Category> Categories { get; set; }
        internal DbSet<CustomWorkout> CustomWorkouts { get; set; }
        internal DbSet<Exercise> Exercises { get; set; }
        internal DbSet<ExerciseCategory> ExerciseCategories { get; set; }
        internal DbSet<Image> Images { get; set; }
        internal DbSet<MotivationSentence> MotivationSentences { get; set; }
        internal DbSet<PredefinedWorkout> PredefinedWorkouts { get; set; }
        internal DbSet<Video> Videos { get; set; }
        internal DbSet<Workout> Workouts { get; set; }
        internal DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        internal DbSet<WorkoutReminder> WorkoutReminders { get; set; }
        internal DbSet<WorkoutReminderDate> WorkoutReminderDates { get; set; }


        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }

    }
}
