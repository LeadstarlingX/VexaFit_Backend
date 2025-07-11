using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;
using Application.IUnitOfWork;
using Application.Serializer;
using Domain.Entities.IdentityEntites;
using Infrastructure.Context;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyInjection
    {

        /// <summary>
        /// Adds the infrastructure.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddIdentityOptions();

        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppRepository<>), typeof(Repository.AppRepository<>));
            services.AddScoped(typeof(IIdentityAppRepository<>), typeof(IdentityRepository<>));
            services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();
            //services.AddScoped<DataSeeder>();
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }

        /// <summary>
        /// Adds the identity options.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        private static IServiceCollection AddIdentityOptions(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            return services;
        }

        /// <summary>
        /// Adds the database.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection")));

            return services;
        }
    }
}
