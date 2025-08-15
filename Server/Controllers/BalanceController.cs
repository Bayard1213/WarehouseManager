using Microsoft.AspNetCore.Mvc;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Balance;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly IBalanceService _balanceService;

        public BalanceController(ILogger<BalanceController> logger, IBalanceService balanceService)
        {
            _balanceService = balanceService;
            _logger = logger;
        }
        /// <summary>
        /// Получить список баланса с фильтрацией
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BalanceFilterDto filter)
        {
            var balances = await _balanceService.GetAllAsync(filter);
            return Ok(new ApiResponse<IEnumerable<VBalanceDto>>(
                "Список остатков по складу получен",
                balances
            ));
        }

        /// <summary>
        /// Получить баланс по конкретному ресурсу и единице измерения
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        [HttpGet("{resourceId:int}/{measureId:int}")]
        public async Task<IActionResult> GetByIds(int resourceId, int measureId)
        {
            var balance = await _balanceService.GetByIdsAsync(resourceId, measureId);
            if (balance is null)
                return NotFound(new ApiResponse("Баланс по данным параметрам не найден", false));

            return Ok(new ApiResponse<VBalanceDto>(
                $"Баланс по ресурсу '{balance.ResourceName}' и единице измерения '{balance.MeasureName}' получен",
                balance
            ));
        }
        /// <summary>
        /// Проверить наличие достаточного количества ресурсов
        /// </summary>
        /// <param name="requiredResources"></param>
        /// <returns></returns>
        [HttpPost("has-enough")]
        public async Task<IActionResult> HasEnough([FromBody] IEnumerable<ResourceQuantityDto> requiredResources)
        {
            var hasEnough = await _balanceService.HasEnoughAsync(requiredResources);
            return Ok(new ApiResponse<bool>(
                hasEnough ? "Ресурсов достаточно" : "Ресурсов недостаточно",
                hasEnough
            ));
        }
    }
}
