using Microsoft.AspNetCore.Mvc;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Dtos.Resource;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly ILogger<ResourceController> _logger;
        private readonly IResourceService _resourceService;

        public ResourceController(ILogger<ResourceController> logger, IResourceService resourceService)
        {
            _resourceService = resourceService;
            _logger = logger;
        }
        /// <summary>
        /// Получить список всех ресурсов
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var resources = await _resourceService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<ResourceDto>>("Полный список ресурсов", resources));
        }

        /// <summary>
        /// Получить список всех ресурсов (только активные/все)
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFiltered([FromQuery] bool isActive = true)
        {
            var resources = await _resourceService.GetAllFilteredAsync(isActive);
            return Ok(new ApiResponse<IEnumerable<ResourceDto>>(
                isActive ? "Список активных ресурсов" : "Список клиентов из архива",
                resources
            ));
        }
        /// <summary>
        /// Получить информацию по конкретному ресурсу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null)
                return NotFound(new ApiResponse($"Ресурс с Id={id} не найден", false));

            return Ok(new ApiResponse<ResourceDto>(
                $"Ресурс '{resource.Name}' успешно получен",
                resource
            ));
        }
        /// <summary>
        /// Создать ресурс
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResourceDto dto)
        {
            var createdResource = await _resourceService.CreateAsync(dto);
            return Ok(new ApiResponse<ResourceDto>(
                $"Клиент '{createdResource.Name}' успешно создан",
                createdResource
            ));
        }
        /// <summary>
        /// Обновить ресурс
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResourceDto dto)
        {
            var name = await _resourceService.UpdateAsync(id, dto);
            return Ok(new ApiResponse($"Ресурс '{name}' успешно обновлён"));
        }
        /// <summary>
        /// Удалить ресурс
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var name = await _resourceService.DeleteAsync(id);
            return Ok(new ApiResponse($"Ресурс '{name}' успешно удалён"));
        }
        /// <summary>
        /// Отправить ресурс в архив
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var name = await _resourceService.ArchiveAsync(id);
            return Ok(new ApiResponse($"Ресурс '{name}' успешно архивирован"));
        }
        /// <summary>
        /// Убрать ресурс из архива
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/unarchive")]
        public async Task<IActionResult> Unarchive(int id)
        {
            var name = await _resourceService.UnarchiveAsync(id);
            return Ok(new ApiResponse($"Ресурс '{name}' успешно убран из архива"));
        }
        /// <summary>
        /// Проверить имя на уникальность
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckNameUnique([FromQuery] string name)
        {
            bool isUnique = await _resourceService.CheckNameUniqueAsync(name);
            return Ok(new ApiResponse<bool>(
                isUnique ? "Имя ресурса свободно" : "Ресурс с таким названием уже существует",
                isUnique
            ));
        }
    }
}
