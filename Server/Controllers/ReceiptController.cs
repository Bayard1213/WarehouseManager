using Microsoft.AspNetCore.Mvc;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly ILogger<ReceiptController> _logger;
        private readonly IReceiptService _receiptService;

        public ReceiptController(ILogger<ReceiptController> logger, IReceiptService receiptService)
        {
            _receiptService = receiptService;
            _logger = logger;
        }

        /// <summary>
        /// Получить список документов поступления с ресурсами с фильтрацией
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ReceiptFilterDto filter)
        {
            var receipts = await _receiptService.GetAllAsync(filter);
            return Ok(new ApiResponse<IEnumerable<ReceiptWithResourcesDto>>(
                "Список документов поступления получен",
                receipts
            ));
        }
        /// <summary>
        /// Получить информацию по конкретному документу поступления
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt == null)
                return NotFound(new ApiResponse($"Документ поступления с Id={id} не найден", false));

            return Ok(new ApiResponse<ReceiptWithResourcesDto>(
                $"Документ поступления №{receipt.DocumentNumber} получен",
                receipt
            ));
        }
        /// <summary>
        /// Содать документ поступления
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceiptDto dto)
        {
            var created = await _receiptService.CreateAsync(dto);
            return Ok(new ApiResponse<ReceiptWithResourcesDto>(
                $"Документ поступления №{created.DocumentNumber} успешно создан",
                created
            ));
        }
        /// <summary>
        /// Обновить документ поступления
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReceiptDto dto)
        {
            var number = await _receiptService.UpdateAsync(id, dto);
            return Ok(new ApiResponse($"Документ поступления № {number} успешно обновлён"));
        }
        /// <summary>
        /// Удалить документ поступления
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var number = await _receiptService.DeleteAsync(id);
            return Ok(new ApiResponse($"Документ поступления № {number} успешно удалён"));
        }

        /// <summary>
        /// Проверить имя на уникальность
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckNumberUnique([FromQuery] string name)
        {
            bool isUnique = await _receiptService.CheckNumberUniqueAsync(name);
            return Ok(new ApiResponse<bool>(
                isUnique ? "Номер документа свободен" : "Документ с таким номером уже существует",
                isUnique
            ));
        }
    }
}
