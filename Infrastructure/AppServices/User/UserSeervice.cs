using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.IAppServices.User;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AppServices.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<AdminDashboardStatsDTO> GetDashboardStatsAsync()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalWorkouts = await _context.Workouts.CountAsync();
            var totalExercises = await _context.Exercises.CountAsync();

            // ✨ IMPLEMENTED: Calculate active users from the last 24 hours
            var today = DateTime.UtcNow.Date;
            var activeUsersToday = await _userManager.Users
                .CountAsync(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value.Date == today);

            return new AdminDashboardStatsDTO
            {
                TotalUsers = totalUsers,
                TotalWorkouts = totalWorkouts,
                TotalExercises = totalExercises,
                ActiveUsersToday = activeUsersToday
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    JoinedDate = user.CreatedAt,
                    Roles = roles
                });
            }

            return userDtos;
        }

        public async Task ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Toggle the IsActive property
            user.IsActive = !user.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Combine errors into a single exception message
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user status: {errors}");
            }
        }
    }
}
