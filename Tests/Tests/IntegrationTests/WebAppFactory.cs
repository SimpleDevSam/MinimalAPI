using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var cwd = Directory.GetCurrentDirectory();
            // cwd might be "/home/runner/.../Tests/Tests/bin/Debug/net8.0"
            // Walk up four levels, then down into "MinimalAPI/MinimalAPI"
            var projectRoot = Path.GetFullPath(Path.Combine(
                cwd,
                "..",   // up to net8.0
                "..",   // up to bin
                "..",   // up to Tests/Tests
                "..",   // up to MinimalAPI/MinimalAPI (level 2)
                "MinimalAPI", // now level 3
                "MinimalAPI"  // now level 4 → actual WebAPI folder
            ));
            Console.WriteLine($"[Diagnostics] Computed API folder: {projectRoot}");
            builder.UseContentRoot(projectRoot);


            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            Console.WriteLine("[Diagnostics] In-memory SQLite: db.Database.Migrate() ran");


            services.AddSingleton(_connection);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
            Console.WriteLine("[Diagnostics] Disposed in-memory SQLite connection");

        }

        base.Dispose(disposing);
    }
}

