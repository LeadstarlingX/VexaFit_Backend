using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds
{
    public class DataSeeder(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManeger,
        UserManager<ApplicationUser> userManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManeger = roleManeger;
        public void SeedData()
        {
            var shouldUpdateContext = false;


            if (shouldUpdateContext)
            {
                shouldUpdateContext = false;
                _context.SaveChanges();
            }
            if (!context.Roles.Any())
            {
                shouldUpdateContext = true;

                var roleUser = new ApplicationRole()
                {
                    Name = DefaultSettings.TraineeRoleName,
                };
                var roleAdmin = new ApplicationRole()
                {
                    Name = DefaultSettings.AdminRoleName
                };

                _roleManeger.CreateAsync(roleAdmin).GetAwaiter().GetResult();
                _roleManeger.CreateAsync(roleUser).GetAwaiter().GetResult();
            }

            if (!context.Users.Any(u => u.Email == DefaultSettings.DefaultAdminOneEmail))
            {
                var adminUser = new ApplicationUser
                {
                    Email = DefaultSettings.DefaultAdminOneEmail,
                    UserName = DefaultSettings.DefaultAdminOneUserName,
                    PhoneNumber = DefaultSettings.DefaultAdminOnePhone,
                    PhoneNumberConfirmed = true,
                    IsActive = true
                };

                var result = _userManager.CreateAsync(adminUser, DefaultSettings.DefaultAdminPassword).GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(adminUser, DefaultSettings.AdminRoleName).GetAwaiter().GetResult();
                    var code = _userManager.GenerateEmailConfirmationTokenAsync(adminUser).GetAwaiter().GetResult();
                    _userManager.ConfirmEmailAsync(adminUser, code).GetAwaiter().GetResult();
                }

                shouldUpdateContext = true;
            }
            if (shouldUpdateContext)
            {
                context.SaveChanges();
                shouldUpdateContext = false;
            }
            var identityUser = context.Users.FirstOrDefault(u => u.Email == DefaultSettings.DefaultAdminOneEmail);
            if (identityUser == null)
            {
                throw new InvalidOperationException("Admin user not found after seeding.");
            }

            var adminUserId = identityUser.Id;
        }
    }
}
