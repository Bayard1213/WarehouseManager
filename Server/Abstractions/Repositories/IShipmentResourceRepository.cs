using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IShipmentResourceRepository : IGenericRepository<ShipmentResource>
    {
        Task<ICollection<ShipmentResource>> GetByDocumentIdAsync(int documentShipmentId);
        Task DeleteByDocumentIdAsync(int documentShipmentId);
    }
}
