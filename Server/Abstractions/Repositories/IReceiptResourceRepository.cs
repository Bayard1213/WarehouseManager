using System.Runtime.CompilerServices;
using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IReceiptResourceRepository : IGenericRepository<ReceiptResource>
    {
        Task<ICollection<ReceiptResource>> GetByDocumentIdAsync(int documentReceiptId);
        Task DeleteByDocumentIdAsync(int documentReceiptId);
    }
}
