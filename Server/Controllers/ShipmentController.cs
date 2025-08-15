using Microsoft.AspNetCore.Mvc;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly ILogger<ShipmentController> _logger;
        private readonly IShipmentService _shipmentService;

        public ShipmentController(ILogger<ShipmentController> logger, IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
            _logger = logger;
        }
        /// <summary>
        /// Получить список документов отгрузки с ресурсами с фильтрацией 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ShipmentFilterDto filter)
        {
            var list = await _shipmentService.GetAllAsync(filter);
            return Ok(new ApiResponse<IEnumerable<ShipmentWithResourcesDto>>(
                "Список отгрузок получен", list
            ));
        }
        /// <summary>
        /// Получить информацию по конкретному документу отгрузки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
                return NotFound(new ApiResponse("Отгрузка не найдена", false));

            return Ok(new ApiResponse<ShipmentWithResourcesDto>(
                $"Отгрузка №{shipment.DocumentNumber} получена", shipment
            ));
        }
        /// <summary>
        /// Содать документ отгрузки
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShipmentDto dto)
        {
            var number = await _shipmentService.CreateAsync(dto);
            return Ok(new ApiResponse($"Отгрузка №{number} успешно создана"));
        }
        /// <summary>
        /// Обновить документ отгрузки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShipmentDto dto)
        {
            var number = await _shipmentService.UpdateAsync(id, dto);
            return Ok(new ApiResponse($"Отгрузка №{number} успешно создана"));
        }
        /// <summary>
        /// Удалить документ отгрузки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var number = await _shipmentService.DeleteAsync(id);
            return Ok(new ApiResponse($"Отгрузка №{number} успешно создана"));
        }
        /// <summary>
        /// Изменить статус документа отгрузки на "Подписан"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/sign")]
        public async Task<IActionResult> Sign(int id)
        {
            var number = await _shipmentService.SignAsync(id);
            return Ok(new ApiResponse($"Отгрузка №{number} успешно создана"));
        }
        /// <summary>
        /// Изменить статус документа отгрузки на "Отозван"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/revoke")]
        public async Task<IActionResult> Revoke(int id)
        {
            var number = await _shipmentService.RevokeAsync(id);
            return Ok(new ApiResponse($"Подпись отгрузки №{number} успешно создана"));
        }

        /// <summary>
        /// Проверить имя на уникальность
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckNumberUnique([FromQuery] string name)
        {
            bool isUnique = await _shipmentService.CheckNumberUniqueAsync(name);
            return Ok(new ApiResponse<bool>(
                isUnique ? "Номер документа свободен" : "Документ с таким номером уже существует",
                isUnique
            ));
        }
    }
}
