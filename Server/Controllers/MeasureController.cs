using Microsoft.AspNetCore.Mvc;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Dtos.Measure;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasureController : ControllerBase
    {
        private readonly ILogger<MeasureController> _logger;
        private readonly IMeasureService _measureService;

        public MeasureController(ILogger<MeasureController> logger, IMeasureService measureService)
        {
            _measureService = measureService;
            _logger = logger;
        }

        /// <summary>
        /// Получить список всех единиц измерения
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var measures = await _measureService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<MeasureDto>>( "Список всех единиц измерения из архива", measures));
        }

        /// <summary>
        /// Получить список всех единиц измерения (только активные/все)
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFiltered([FromQuery] bool isActive = true)
        {
            var measures = await _measureService.GetAllFilteredAsync(isActive);
            return Ok(new ApiResponse<IEnumerable<MeasureDto>>(
                isActive ? "Список активных единиц измерения" : "Список единиц измерения из архива",
                measures
            ));
        }
        /// <summary>
        /// Получить информацию по конкретной единице измерения
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var measure = await _measureService.GetByIdAsync(id);
            if (measure == null)
                return NotFound(new ApiResponse($"Единица измерения с Id={id} не найдена"));
            
            return Ok(new ApiResponse<MeasureDto>(
                $"Единица измерения '{measure.Name}' успешно получена",
                measure
            ));
        }
        /// <summary>
        /// Создать единицу измерения
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeasureDto dto)
        {
            var createdMeasure = await _measureService.CreateAsync(dto);
            return Ok(new ApiResponse<MeasureDto>(
                $"Единица измерения '{createdMeasure.Name}' успешно создана",
                createdMeasure
            ));
        }
        /// <summary>
        /// Обновить единицу измерения
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMeasureDto dto)
        {
            var name = await _measureService.UpdateAsync(id, dto);
            return Ok(new ApiResponse($"Единица измерения '{name}' успешно обновлена"));
        }
        /// <summary>
        /// Удалить единицу измерения
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var name = await _measureService.DeleteAsync(id);
            return Ok(new ApiResponse($"Единица измерения '{name}' успешно удалена"));
        }
        /// <summary>
        /// Отправить единицу измерения в архив
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var name = await _measureService.ArchiveAsync(id);
            return Ok(new ApiResponse($"Единица измерения '{name}' успешно архивирована"));
        }
        /// <summary>
        /// Убрать единицу измерения из архива
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/unarchive")]
        public async Task<IActionResult> Unarchive(int id)
        {
            var name = await _measureService.UnarchiveAsync(id);
            return Ok(new ApiResponse($"Единица измерения '{name}' успешно убрана из архива"));
        }
        /// <summary>
        /// Проверить имя на уникальность
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckNameUnique([FromQuery] string name)
        {
            bool isUnique = await _measureService.CheckNameUniqueAsync(name);
            return Ok(new ApiResponse<bool>(
                isUnique ? "Имя единицы измерения свободно" : "Единица измерения с таким названием уже существует",
                isUnique
            ));
        }
    }
}
