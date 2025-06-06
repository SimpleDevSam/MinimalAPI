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
            Console.WriteLine($"[Diagnostics] Test CWD: {cwd}");

            // ─── STEP 2: Climb up folders until we find "MinimalAPI.csproj" ───
            // We assume your WebAPI project file is named "MinimalAPI.csproj".
            // If yours is named differently, change the string below.
            var dir = new DirectoryInfo(cwd);
            DirectoryInfo? webApiFolder = null;

            while (dir != null)
            {
                // Check for the .csproj file inside this directory:
                if (dir.EnumerateFiles("MinimalAPI.csproj", SearchOption.TopDirectoryOnly).Any())
                {
                    webApiFolder = dir;
                    break;
                }

                dir = dir.Parent;
            }

            if (webApiFolder == null)
            {
                throw new DirectoryNotFoundException(
                    $"Could not find MinimalAPI.csproj by climbing up from '{cwd}'.");
            }

            var projectRoot = webApiFolder.FullName;
            Console.WriteLine($"[Diagnostics] Resolved WebAPI folder (content root): {projectRoot}");

            // ─── STEP 3: Tell ASP.NET to use that folder as content root ───
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

