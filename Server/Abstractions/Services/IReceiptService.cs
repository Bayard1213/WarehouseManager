using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IReceiptService
    {
        Task<IEnumerable<ReceiptWithResourcesDto>> GetAllAsync(ReceiptFilterDto filter);
        Task<ReceiptWithResourcesDto?> GetByIdAsync(int id);
        Task<ReceiptWithResourcesDto> CreateAsync(CreateReceiptDto dto);
        Task<string> UpdateAsync(int id, UpdateReceiptDto dto);
        Task<string> DeleteAsync(int id);
        
        //
        Task<bool> CheckNumberUniqueAsync(string number);
    }
}
