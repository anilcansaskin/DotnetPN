using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetPN.Models;

namespace DotnetPN.Interfaces;

public interface IDeviceRepository : IRepository<Device>
{
    Task<Device?> GetByTokenAsync(string token);
    Task<IReadOnlyList<Device>> GetByUserIdAsync(string userId);
}
