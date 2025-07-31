using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mapping.CategoryProfile;
using Application.Mapping.ExerciseProfile;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddAutoMapper(x => x.AddMaps(typeof(CategoryProfile).Assembly));
            services.AddAutoMapper(x => x.AddMaps(typeof(ExerciseProfile).Assembly));

            return services;
        }
    }
}
