using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotnetPN.Data;
using DotnetPN.Interfaces;
using DotnetPN.Models;

namespace DotnetPN.Repositories;

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Device?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.DeviceToken == token);
    }

    public async Task<IReadOnlyList<Device>> GetByUserIdAsync(string userId)
    {
        return await _dbSet.Where(d => d.UserId == userId).ToListAsync();
    }
}
