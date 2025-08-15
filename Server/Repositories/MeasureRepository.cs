using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Repositories
{
    public class MeasureRepository : GenericRepository<Measure>, IMeasureRepository
    {
        private readonly ILogger<MeasureRepository> _logger;

        public MeasureRepository(AppDbContext dbContext,
            ILogger<GenericRepository<Measure>> genericLogger,
            ILogger<MeasureRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<bool> AnyByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(m => m.Name == name);
        }

        public async Task<IEnumerable<Measure>> GetAllFilteredAsync(bool isActive)
        {
            IQueryable<Measure> query = _dbSet;

            if (isActive)
                query = query.Where(m => m.Status == (int)EntityStatus.Active);
            else
                query = query.Where(m => m.Status == (int)EntityStatus.Archived);

                return await query.ToListAsync();
        }

        public async Task<Measure?> GetByIdWithTrackingAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
