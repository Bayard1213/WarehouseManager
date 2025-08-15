using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<IEnumerable<Client>> GetAllFilteredAsync(bool isActive);
        Task<bool> AnyByNameAsync(string name);
        Task<Client?> GetByIdWithTrackingAsync(int id);
    }
}
