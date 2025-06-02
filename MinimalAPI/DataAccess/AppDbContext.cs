using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MinimalAPI_KeyCloack.Data.Models;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public string DbPath { get; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
}