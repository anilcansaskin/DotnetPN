using Microsoft.EntityFrameworkCore;
using DotnetPN.Models;

namespace DotnetPN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Device>()
            .HasIndex(d => d.DeviceToken)
            .IsUnique();
    }
}
