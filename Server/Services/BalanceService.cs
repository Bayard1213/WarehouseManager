using System.ComponentModel.DataAnnotations;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Shared.Dtos.Balance;

namespace WarehouseManager.Server.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly ILogger<BalanceService> _logger;
        private readonly IBalanceRepository _balanceRepository;

        public BalanceService(ILogger<BalanceService> logger,
            IBalanceRepository balanceRepository)
        {
            _logger = logger;
            _balanceRepository = balanceRepository;
        }
        public async Task<IEnumerable<VBalanceDto>> GetAllAsync(BalanceFilterDto filter)
        {
            var entities = await _balanceRepository.GetAllViewAsync(filter);

            return entities.Select(e => new VBalanceDto
            {
                ResourceId = e.ResourceId,
                ResourceName = e.ResourceName,
                ResourceStatus = e.ResourceStatus,
                MeasureId = e.MeasureId,
                MeasureName = e.MeasureName,
                MeasureStatus = e.MeasureStatus,
                BalanceQuantity = e.BalanceQuantity
            });
        }
        public async Task<VBalanceDto?> GetByIdsAsync(int resourceId, int measureId)
        {
            var entity = await _balanceRepository.GetViewByIdsAsync(resourceId, measureId);
            if (entity is null) return null;

            return new VBalanceDto
            {
                ResourceId = entity.ResourceId,
                ResourceName = entity.ResourceName,
                ResourceStatus = entity.ResourceStatus,
                MeasureId = entity.MeasureId,
                MeasureName = entity.MeasureName,
                MeasureStatus = entity.MeasureStatus,
                BalanceQuantity = entity.BalanceQuantity
            };
        }
        public Task<bool> HasEnoughAsync(IEnumerable<ResourceQuantityDto> requiredResources)
            => _balanceRepository.HasEnoughAsync(requiredResources);

        public Task IncreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToIncrease)
            => _balanceRepository.IncreaseAsync(resourcesToIncrease);

        public Task DecreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToDecrease)
            => _balanceRepository.DecreaseAsync(resourcesToDecrease);
    }
}
