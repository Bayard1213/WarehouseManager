using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Repositories
{
    public class ShipmentResourceRepository : GenericRepository<ShipmentResource>, IShipmentResourceRepository
    {
        private readonly ILogger<ShipmentResourceRepository> _logger;

        public ShipmentResourceRepository(AppDbContext dbContext,
            ILogger<GenericRepository<ShipmentResource>> genericLogger,
            ILogger<ShipmentResourceRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }
        public async Task<ICollection<ShipmentResource>> GetByDocumentIdAsync(int documentShipmentId)
        {
            return await _dbSet
                        .Where(sr => sr.DocumentShipmentId == documentShipmentId)
                        .ToListAsync();
        }
        public async Task DeleteByDocumentIdAsync(int documentShipmentId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var items = await _dbSet
                    .Where(sr => sr.DocumentShipmentId == documentShipmentId)
                    .ToListAsync();

                _dbSet.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
