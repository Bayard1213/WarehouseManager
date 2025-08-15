using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IShipmentService
    {
        Task<IEnumerable<ShipmentWithResourcesDto>> GetAllAsync(ShipmentFilterDto filter);
        Task<ShipmentWithResourcesDto?> GetByIdAsync(int id);
        Task<ShipmentWithResourcesDto> CreateAsync(CreateShipmentDto dto);
        Task<string> UpdateAsync(int id, UpdateShipmentDto dto);
        Task<string> DeleteAsync(int id);

        //
        Task<IEnumerable<string>> GetDocumentNumbersAsync();
        Task<bool> CheckNumberUniqueAsync(string number);
        Task<string> SignAsync(int id);
        Task<string> RevokeAsync(int id);
    }
}
