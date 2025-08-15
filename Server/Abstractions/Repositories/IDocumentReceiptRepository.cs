using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IDocumentReceiptRepository : IGenericRepository<DocumentsReceipt>
    {
        Task<IEnumerable<DocumentsReceipt>> GetAllAsync(ReceiptFilterDto filter);
        Task<bool> ExistByNumberAsync(string number);
        Task<IEnumerable<string>> GetAllNumberAsync();
        Task<IEnumerable<VReceiptDocumentResource>> GetResourcesForDocumentAsync(int documentReceiptId);
        Task UpdateWithResourcesAsync(DocumentsReceipt entity, UpdateReceiptDto dto);

    }
}
