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
            var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
            Directory.CreateDirectory(dbFolder); 


            var dbPath = Path.Combine(dbFolder, "app.db");

            Console.WriteLine("Using SQLite DB at: " + dbPath); 

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
