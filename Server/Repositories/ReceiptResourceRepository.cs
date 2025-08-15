using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Repositories
{
    public class ReceiptResourceRepository : GenericRepository<ReceiptResource>, IReceiptResourceRepository
    {
        private readonly ILogger<ReceiptResourceRepository> _logger;

        public ReceiptResourceRepository(AppDbContext dbContext,
            ILogger<GenericRepository<ReceiptResource>> genericLogger,
            ILogger<ReceiptResourceRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<ICollection<ReceiptResource>> GetByDocumentIdAsync(int documentReceiptId)
        {
            return await _dbSet
                .Where(rr => rr.DocumentReceiptId == documentReceiptId)
                .ToListAsync();
        }
        public async Task DeleteByDocumentIdAsync(int documentReceiptId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var items = await _dbSet
                    .Where(rr => rr.DocumentReceiptId == documentReceiptId)
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
