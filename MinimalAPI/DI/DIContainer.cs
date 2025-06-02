using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using MinimalAPI_KeyCloack.Application.Validators;

namespace MinimalAPI_KeyCloack.DependencyInjection
{
    public static class DIContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register services from the Application layer
            return services;
        }

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={config.GetConnectionString("DefaultConnection")}"));

            return services;
        }
        public static IServiceCollection AddValidatorsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateUserValidator>();
            return services;
        }
    }
}
