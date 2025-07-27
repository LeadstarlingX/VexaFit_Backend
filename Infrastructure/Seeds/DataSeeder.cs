using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.AppEntities;
using Domain.Enum;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeds
{
    public class DataSeeder(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManeger,
        UserManager<ApplicationUser> userManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManeger;
        public async Task SeedAllAsync()
        {
            if (await _context.Database.CanConnectAsync())
            {
                await SeedRolesAsync();
                await SeedAdminUserAsync();
                await SeedTraineeUsersAsync(); // Updated method name
                await ClearAndReseedContentAsync();
            }
        }

        private async Task ClearAndReseedContentAsync()
        {
            // Clears and reseeds content data for development
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"WorkoutExercises\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Workouts\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"ExerciseCategories\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Exercises\"");

            await SeedExercisesAndCategoriesAsync();
            await SeedWorkoutsAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = DefaultSettings.AdminRoleName });
                await _roleManager.CreateAsync(new ApplicationRole { Name = DefaultSettings.TraineeRoleName });
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Email == DefaultSettings.DefaultAdminOneEmail))
            {
                var adminUser = new ApplicationUser
                {
                    Email = DefaultSettings.DefaultAdminOneEmail,
                    UserName = DefaultSettings.DefaultAdminOneUserName,
                    PhoneNumber = DefaultSettings.DefaultAdminOnePhone,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(adminUser, DefaultSettings.DefaultAdminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, DefaultSettings.AdminRoleName);
                }
            }
        }

        private async Task SeedTraineeUsersAsync() // Renamed and expanded
        {
            // Trainee 1
            if (!await _context.Users.AnyAsync(u => u.Email == DefaultSettings.DefaultTraineeEmail))
            {
                var traineeUser = new ApplicationUser
                {
                    Email = DefaultSettings.DefaultTraineeEmail,
                    UserName = DefaultSettings.DefaultTraineeUserName,
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(traineeUser, DefaultSettings.DefaultTraineePassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(traineeUser, DefaultSettings.TraineeRoleName);
                }
            }
            // Trainee 2 (New)
            if (!await _context.Users.AnyAsync(u => u.Email == "trainee2@app.com"))
            {
                var traineeUser = new ApplicationUser
                {
                    Email = "trainee2@app.com",
                    UserName = "TraineeTwo",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(traineeUser, "P@ssword123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(traineeUser, DefaultSettings.TraineeRoleName);
                }
            }
            // Trainee 3 (New)
            if (!await _context.Users.AnyAsync(u => u.Email == "trainee3@app.com"))
            {
                var traineeUser = new ApplicationUser
                {
                    Email = "trainee3@app.com",
                    UserName = "TraineeThree",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(traineeUser, "P@ssword123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(traineeUser, DefaultSettings.TraineeRoleName);
                }
            }
        }

        private async Task SeedExercisesAndCategoriesAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Chest", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Legs", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Core", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Full Body", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Strength", Type = CategoryTypeEnum.ExerciseType },
                    new Category { Name = "Cardio", Type = CategoryTypeEnum.ExerciseType },
                    new Category { Name = "Standing", Type = CategoryTypeEnum.Position },
                    new Category { Name = "Lying Down", Type = CategoryTypeEnum.Position }
                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }

            if (!await _context.Exercises.AnyAsync())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise { Name = "Push-up", Description = "A classic bodyweight exercise." },
                    new Exercise { Name = "Squat", Description = "A fundamental lower body exercise." },
                    new Exercise { Name = "Plank", Description = "An isometric core strength exercise." },
                    new Exercise { Name = "Jumping Jacks", Description = "A full-body cardio exercise." },
                    new Exercise { Name = "Burpees", Description = "A full-body exercise used in strength training and as an aerobic exercise." }
                };
                await _context.Exercises.AddRangeAsync(exercises);
                await _context.SaveChangesAsync();
            }

            if (!await _context.ExerciseCategories.AnyAsync())
            {
                var exercisesTask = _context.Exercises.ToDictionaryAsync(e => e.Name, e => e);
                var categoriesTask = _context.Categories.ToDictionaryAsync(c => c.Name, c => c);

                await Task.WhenAll(exercisesTask, categoriesTask);

                var exercises = exercisesTask.Result;
                var categories = categoriesTask.Result;

                var exerciseCategories = new List<ExerciseCategory>
                {
                    new ExerciseCategory { ExerciseId = exercises["Push-up"].Id, CategoryId = categories["Chest"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Push-up"].Id, CategoryId = categories["Strength"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Squat"].Id, CategoryId = categories["Legs"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Squat"].Id, CategoryId = categories["Strength"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Plank"].Id, CategoryId = categories["Core"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Plank"].Id, CategoryId = categories["Strength"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Jumping Jacks"].Id, CategoryId = categories["Cardio"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Jumping Jacks"].Id, CategoryId = categories["Standing"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Burpees"].Id, CategoryId = categories["Full Body"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Burpees"].Id, CategoryId = categories["Cardio"].Id },
                    new ExerciseCategory { ExerciseId = exercises["Burpees"].Id, CategoryId = categories["Strength"].Id }
                };
                await _context.ExerciseCategories.AddRangeAsync(exerciseCategories);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedWorkoutsAsync()
        {
            if (!await _context.Workouts.AnyAsync())
            {
                var adminUser = await _context.Users.FirstAsync(u => u.Email == DefaultSettings.DefaultAdminOneEmail);
                var traineeUser1 = await _context.Users.FirstAsync(u => u.Email == DefaultSettings.DefaultTraineeEmail);
                var traineeUser2 = await _context.Users.FirstAsync(u => u.Email == "trainee2@app.com");
                var traineeUser3 = await _context.Users.FirstAsync(u => u.Email == "trainee3@app.com");


                var workouts = new List<Workout>
                {
                    // Predefined Workouts (More added)
                    new PredefinedWorkout { Name = "Full Body Strength", Description = "A simple workout to target all major muscle groups." },
                    new PredefinedWorkout { Name = "Quick Cardio Blast", Description = "A 5-minute cardio workout." },
                    new PredefinedWorkout { Name = "Core Focus", Description = "Strengthen your core with this targeted routine." },
                    new PredefinedWorkout { Name = "Leg Day Basics", Description = "Foundational exercises for lower body strength." },
                    
                    // Custom Workouts (More added and assigned)
                    new CustomWorkout { Name = "Admin's Core Routine", Description = "A custom workout by the admin.", UserId = adminUser.Id, CreationDate = DateTime.UtcNow },
                    new CustomWorkout { Name = "Trainee1 First Workout", Description = "A custom workout created by the first trainee.", UserId = traineeUser1.Id, CreationDate = DateTime.UtcNow },
                    new CustomWorkout { Name = "Trainee1 Cardio Day", Description = "A second custom workout for the first trainee.", UserId = traineeUser1.Id, CreationDate = DateTime.UtcNow.AddDays(-1) },
                    new CustomWorkout { Name = "Trainee2 Leg Power", Description = "Trainee two's lower body day.", UserId = traineeUser2.Id, CreationDate = DateTime.UtcNow },
                    new CustomWorkout { Name = "Trainee3 Full Body Intro", Description = "Trainee three's introduction to strength training.", UserId = traineeUser3.Id, CreationDate = DateTime.UtcNow }
                };
                await _context.Workouts.AddRangeAsync(workouts);
                await _context.SaveChangesAsync();
            }

            if (!await _context.WorkoutExercises.AnyAsync())
            {
                var workouts = await _context.Workouts.ToDictionaryAsync(w => w.Name, w => w);
                var exercises = await _context.Exercises.ToDictionaryAsync(e => e.Name, e => e);

                var workoutExercises = new List<WorkoutExercise>
                {
                    // Full Body Strength
                    new WorkoutExercise { WorkoutId = workouts["Full Body Strength"].Id, ExerciseId = exercises["Squat"].Id, Sets = 3, Reps = 12, WeightKg = 20 },
                    new WorkoutExercise { WorkoutId = workouts["Full Body Strength"].Id, ExerciseId = exercises["Push-up"].Id, Sets = 3, Reps = 15 },
                    
                    // Quick Cardio Blast
                    new WorkoutExercise { WorkoutId = workouts["Quick Cardio Blast"].Id, ExerciseId = exercises["Jumping Jacks"].Id, Sets = 1, DurationSeconds = 180 },
                    new WorkoutExercise { WorkoutId = workouts["Quick Cardio Blast"].Id, ExerciseId = exercises["Burpees"].Id, Sets = 1, DurationSeconds = 120 },
                    
                    // Core Focus
                    new WorkoutExercise { WorkoutId = workouts["Core Focus"].Id, ExerciseId = exercises["Plank"].Id, Sets = 4, Reps = 1, DurationSeconds = 45 },
                    
                    // Leg Day Basics
                    new WorkoutExercise { WorkoutId = workouts["Leg Day Basics"].Id, ExerciseId = exercises["Squat"].Id, Sets = 4, Reps = 10, WeightKg = 50 },

                    // Admin's Core Routine
                    new WorkoutExercise { WorkoutId = workouts["Admin's Core Routine"].Id, ExerciseId = exercises["Plank"].Id, Sets = 3, Reps = 1, DurationSeconds = 90 },
                    
                    // Trainee1 First Workout
                    new WorkoutExercise { WorkoutId = workouts["Trainee1 First Workout"].Id, ExerciseId = exercises["Squat"].Id, Sets = 2, Reps = 10 },
                    
                    // Trainee1 Cardio Day
                    new WorkoutExercise { WorkoutId = workouts["Trainee1 Cardio Day"].Id, ExerciseId = exercises["Burpees"].Id, Sets = 3, Reps = 10 },
                    new WorkoutExercise { WorkoutId = workouts["Trainee1 Cardio Day"].Id, ExerciseId = exercises["Jumping Jacks"].Id, Sets = 3, Reps = 25 },
                    
                    // Trainee2 Leg Power
                    new WorkoutExercise { WorkoutId = workouts["Trainee2 Leg Power"].Id, ExerciseId = exercises["Squat"].Id, Sets = 5, Reps = 5, WeightKg = 60 },

                    // Trainee3 Full Body Intro
                    new WorkoutExercise { WorkoutId = workouts["Trainee3 Full Body Intro"].Id, ExerciseId = exercises["Push-up"].Id, Sets = 3, Reps = 8 },
                    new WorkoutExercise { WorkoutId = workouts["Trainee3 Full Body Intro"].Id, ExerciseId = exercises["Squat"].Id, Sets = 3, Reps = 10 },
                    new WorkoutExercise { WorkoutId = workouts["Trainee3 Full Body Intro"].Id, ExerciseId = exercises["Plank"].Id, Sets = 3, Reps = 1, DurationSeconds = 30 },
                };
                await _context.WorkoutExercises.AddRangeAsync(workoutExercises);
                await _context.SaveChangesAsync();
            }
        }
    }
}