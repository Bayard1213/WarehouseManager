using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Balance;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IBalanceRepository : IGenericRepository<Balance>
    {
        Task<IEnumerable<VBalance>> GetAllViewAsync(BalanceFilterDto filter);
        Task<VBalance?> GetViewByIdsAsync(int resourceId, int measureId);

        Task<Balance?> GetByIdsAsync(int resourceId, int measureId);

        Task<bool> HasEnoughAsync(IEnumerable<ResourceQuantityDto> requiredResources);

        Task IncreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToIncrease);
        Task DecreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToDecrease);
    }
}
