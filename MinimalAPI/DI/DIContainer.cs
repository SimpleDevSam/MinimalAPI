using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using MinimalAPI_KeyCloack.Application.Validators;
using System;

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
            Console.WriteLine("AppSettings Line SQLite DB at: " + config.GetConnectionString("DefaultConnection"));
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection")));

            return services;
        }
        public static IServiceCollection AddValidatorsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateUserValidator>();
            return services;
        }
    }
}
