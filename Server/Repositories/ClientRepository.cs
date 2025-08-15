using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(AppDbContext dbContext, 
            ILogger<GenericRepository<Client>> genericLogger,
            ILogger<ClientRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<bool> AnyByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Client>> GetAllFilteredAsync(bool isActive)
        {
            IQueryable<Client> query = _dbSet;

            if (isActive)
                query = query.Where(c => c.Status == (int)EntityStatus.Active);
            else
                query = query.Where(c => c.Status == (int)EntityStatus.Archived);

            return await query.ToListAsync();
        }

        public async Task<Client?> GetByIdWithTrackingAsync(int clientId)
        {
            return await _dbSet.FindAsync(clientId);
        }
    }
}
