using WarehouseManager.Shared.Dtos.Balance;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IBalanceService
    {
        Task<IEnumerable<VBalanceDto>> GetAllAsync(BalanceFilterDto filter);
        Task<VBalanceDto?> GetByIdsAsync(int resourceId, int measureId);
        Task<bool> HasEnoughAsync(IEnumerable<ResourceQuantityDto> requiredResources);

        Task IncreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToIncrease);
        Task DecreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToDecrease);
    }

}
