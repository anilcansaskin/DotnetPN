using System.Threading.Tasks;
using DotnetPN.Models;
using DotnetPN.Repositories;

namespace DotnetPN.Interfaces;

/// <summary>
/// Unit of Work contract to coordinate repositories and commit changes.
/// </summary>
public interface IUnitOfWork : System.IDisposable
{
    IRepository<TodoItem> TodoItems { get; }
    IDeviceRepository Devices { get; }
    Task<int> SaveChangesAsync();
}
