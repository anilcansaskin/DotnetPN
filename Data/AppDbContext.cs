using Microsoft.EntityFrameworkCore;
using DotnetPN.Models;

namespace DotnetPN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
