using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.AppEntities;
using Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        #region Tables
        internal DbSet<Achievement> Achievements { get; set; }
        internal DbSet<Category> Categories { get; set; }
        internal DbSet<Exercise> Exercise { get; set; }
        internal DbSet<ExercisePosition> ExercisePositions { get; set; }
        internal DbSet<Food> Foods { get; set; }
        internal DbSet<Image> Images { get; set; }
        internal DbSet<MotivationSentence> MotivationSentences { get; set; }
        internal DbSet<UserHistory> UserHistories { get; set; }
        internal DbSet<UserMeal> UserMeals { get; set; }
        internal DbSet<UserReminder> UserReminders { get; set; }
        internal DbSet<Video> Videos { get; set; }
        internal DbSet<Workout> Workouts { get; set; }
        internal DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.AchievementId);
                entity.ToTable("Achievements");
            });
        }

    }
}
