using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Receipt;
using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IDocumentShipmentRepository : IGenericRepository<DocumentsShipment>
    {
        Task<IEnumerable<DocumentsShipment>> GetAllAsync(ShipmentFilterDto filter);
        Task<IEnumerable<VShipmentDocumentResource>> GetResourcesForDocumentAsync(int documentShipmentId);

        Task<bool> ExistsByNumberAsync(string number);
        Task<IEnumerable<string>> GetAllNumbersAsync();
        Task UpdateWithResourcesAsync(DocumentsShipment entity, UpdateShipmentDto dto);
        Task<DocumentsShipment?> GetByIdWithClientAsync(int id);
    }
}
