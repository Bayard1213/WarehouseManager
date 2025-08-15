using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IMeasureRepository : IGenericRepository<Measure>
    {
        Task<bool> AnyByNameAsync(string name);
        Task<IEnumerable<Measure>> GetAllFilteredAsync(bool isActive);
        Task<Measure?> GetByIdWithTrackingAsync(int id);
    }
}
