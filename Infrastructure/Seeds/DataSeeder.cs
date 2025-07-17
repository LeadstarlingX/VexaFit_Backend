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
            // Using a single transaction for all seeding operations is more efficient.
            if (await _context.Database.CanConnectAsync())
            {
                await SeedRolesAsync();
                await SeedAdminUserAsync();
                await SeedExercisesAndCategoriesAsync();
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!_context.Roles.Any())
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
                    EmailConfirmed = true, // Confirm email directly
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(adminUser, DefaultSettings.DefaultAdminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, DefaultSettings.AdminRoleName);
                }
            }
        }

        private async Task SeedExercisesAndCategoriesAsync()
        {
            // Step 1: Seed Categories if they don't exist
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    // Muscle Groups
                    new Category { Name = "Chest", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Back", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Legs", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Shoulders", Type = CategoryTypeEnum.MuscleGroup },
                    new Category { Name = "Core", Type = CategoryTypeEnum.MuscleGroup },
                    // Exercise Types
                    new Category { Name = "Strength", Type = CategoryTypeEnum.ExerciseType },
                    new Category { Name = "Cardio", Type = CategoryTypeEnum.ExerciseType },
                    new Category { Name = "Stretching", Type = CategoryTypeEnum.ExerciseType },
                    // Positions
                    new Category { Name = "Standing", Type = CategoryTypeEnum.Position },
                    new Category { Name = "Lying Down", Type = CategoryTypeEnum.Position },
                    new Category { Name = "Seated", Type = CategoryTypeEnum.Position }
                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync(); // Save here to get Category IDs
            }

            // Step 2: Seed Exercises if they don't exist
            if (!await _context.Exercises.AnyAsync())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise { Name = "Push-up", Description = "A classic bodyweight exercise targeting the chest, shoulders, and triceps." },
                    new Exercise { Name = "Squat", Description = "A fundamental lower body exercise that targets the quadriceps, hamstrings, and glutes." },
                    new Exercise { Name = "Plank", Description = "An isometric core strength exercise that involves maintaining a position similar to a push-up for the maximum possible time." },
                    new Exercise { Name = "Jumping Jacks", Description = "A full-body cardio exercise that can be done anywhere." }
                };
                await _context.Exercises.AddRangeAsync(exercises);
                await _context.SaveChangesAsync(); // Save here to get Exercise IDs
            }

            // Step 3: Seed the relationship in the junction table
            if (!await _context.ExerciseCategories.AnyAsync())
            {
                // Retrieve the entities we just created to get their IDs
                var pushup = await _context.Exercises.FirstAsync(e => e.Name == "Push-up");
                var squat = await _context.Exercises.FirstAsync(e => e.Name == "Squat");
                var plank = await _context.Exercises.FirstAsync(e => e.Name == "Plank");
                var jumpingJacks = await _context.Exercises.FirstAsync(e => e.Name == "Jumping Jacks");

                var chestCategory = await _context.Categories.FirstAsync(c => c.Name == "Chest");
                var legsCategory = await _context.Categories.FirstAsync(c => c.Name == "Legs");
                var coreCategory = await _context.Categories.FirstAsync(c => c.Name == "Core");
                var strengthCategory = await _context.Categories.FirstAsync(c => c.Name == "Strength");
                var cardioCategory = await _context.Categories.FirstAsync(c => c.Name == "Cardio");
                var lyingDownCategory = await _context.Categories.FirstAsync(c => c.Name == "Lying Down");
                var standingCategory = await _context.Categories.FirstAsync(c => c.Name == "Standing");

                var exerciseCategories = new List<ExerciseCategory>
                {
                    // Link Push-up to its categories
                    new ExerciseCategory { ExerciseId = pushup.Id, CategoryId = chestCategory.Id },
                    new ExerciseCategory { ExerciseId = pushup.Id, CategoryId = strengthCategory.Id },
                    new ExerciseCategory { ExerciseId = pushup.Id, CategoryId = lyingDownCategory.Id },

                    // Link Squat to its categories
                    new ExerciseCategory { ExerciseId = squat.Id, CategoryId = legsCategory.Id },
                    new ExerciseCategory { ExerciseId = squat.Id, CategoryId = strengthCategory.Id },
                    new ExerciseCategory { ExerciseId = squat.Id, CategoryId = standingCategory.Id },

                    // Link Plank to its categories
                    new ExerciseCategory { ExerciseId = plank.Id, CategoryId = coreCategory.Id },
                    new ExerciseCategory { ExerciseId = plank.Id, CategoryId = strengthCategory.Id },
                    new ExerciseCategory { ExerciseId = plank.Id, CategoryId = lyingDownCategory.Id },

                    // Link Jumping Jacks to its categories
                    new ExerciseCategory { ExerciseId = jumpingJacks.Id, CategoryId = cardioCategory.Id },
                    new ExerciseCategory { ExerciseId = jumpingJacks.Id, CategoryId = standingCategory.Id }
                };

                await _context.ExerciseCategories.AddRangeAsync(exerciseCategories);
                await _context.SaveChangesAsync();
            }
        }
    }
}
