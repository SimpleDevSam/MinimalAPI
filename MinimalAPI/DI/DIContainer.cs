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
            var dbFolder = "/home/data";
            Directory.CreateDirectory(dbFolder); // Make sure folder exists

            var dbPath = Path.Combine(dbFolder, "app.db");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            return services;
        }
        public static IServiceCollection AddValidatorsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateUserValidator>();
            return services;
        }
    }
}
