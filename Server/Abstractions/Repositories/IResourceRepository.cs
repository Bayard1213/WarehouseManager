using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IResourceRepository : IGenericRepository<Resource>
    {
        Task<IEnumerable<Resource>> GetAllFilteredAsync(bool isActive);
        Task<bool> AnyByNameAsync(string name);
        Task<Resource?> GetByIdWithTrackingAsync(int resourceId);
    }
}
