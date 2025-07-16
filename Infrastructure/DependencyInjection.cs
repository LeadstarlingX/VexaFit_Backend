using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.IAppServices.Category;
using Application.IRepository;
using Application.IUnitOfWork;
using Application.Mapping.Category;
using Application.Serializer;
using Infrastructure.AppServices.Category;
using Infrastructure.Context;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddIdentityOptions();

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppRepository<>), typeof(Repository.AppRepository<>));
            services.AddScoped(typeof(IIdentityAppRepository<>), typeof(IdentityRepository<>));
            services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services.AddAutoMapper(x => x.AddMaps(typeof(CategoryProfile).Assembly));
            services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<DataSeeder>();
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }

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

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection")));

            return services;
        }

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            //services.AddHostedService<BookingPaymentCheckService>();
            return services;
        }
    }
}
