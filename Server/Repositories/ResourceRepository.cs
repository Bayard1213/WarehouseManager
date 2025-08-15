using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Repositories
{
    public class ResourceRepository : GenericRepository<Resource>, IResourceRepository
    {
        private readonly ILogger<ResourceRepository> _logger;

        public ResourceRepository(AppDbContext dbContext,
            ILogger<GenericRepository<Resource>> genericLogger,
            ILogger<ResourceRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<bool> AnyByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(m => m.Name == name);
        }

        public async Task<IEnumerable<Resource>> GetAllFilteredAsync(bool isActive)
        {
            IQueryable<Resource> query = _dbSet;

            if (isActive)
                query = query.Where(r => r.Status == (int)EntityStatus.Active);
            else
                query = query.Where(r => r.Status == (int)EntityStatus.Archived);

                return await query.ToListAsync();
        }

        public async Task<Resource?> GetByIdWithTrackingAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
