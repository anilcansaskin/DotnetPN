using DotnetPN.Interfaces;
using DotnetPN.Models;
using DotnetPN.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotnetPN.Data;

/// <summary>
/// Concrete Unit of Work that coordinates repositories and persists changes.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<TodoItem>? _todoItems;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<TodoItem> TodoItems => _todoItems ??= new Repository<TodoItem>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
