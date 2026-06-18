using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetPN.Interfaces;

/// <summary>
/// Generic repository contract for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
