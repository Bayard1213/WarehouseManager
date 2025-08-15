using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Shared.Api;
using WarehouseManager.Shared.Dtos.Client;

namespace WarehouseManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientService _clientService;

        public ClientController(ILogger<ClientController> logger, IClientService clientService)
        {
            _clientService = clientService;
            _logger = logger;
        }
        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<ClientDto>>( "Список всех клиентов", clients));
        }
        /// <summary>
        /// Получить список всех клиентов (только активные/все)
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFiltered([FromQuery] bool isActive = true)
        {
            var clients = await _clientService.GetAllFilteredAsync(isActive);
            return Ok(new ApiResponse<IEnumerable<ClientDto>>(
                isActive ? "Список активных клиентов" : "Список клиентов из архива",
                clients
            ));
        }
        /// <summary>
        /// Получить информацию по конкретному клиенту
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
                return NotFound(new ApiResponse($"Клиент с Id={id} не найден", false));

            return Ok(new ApiResponse<ClientDto>(
                $"Клиент '{client.Name}' успешно получен",
                client
            ));
        }
        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            var createdClient = await _clientService.CreateAsync(dto);
            return Ok(new ApiResponse<ClientDto>(
                $"Клиент '{createdClient.Name}' успешно создан",
                createdClient
            ));
        }
        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto dto)
        {
            var name = await _clientService.UpdateAsync(id, dto);
            return Ok(new ApiResponse($"Клиент '{name}' успешно обновлён"));
        }
        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var name = await _clientService.DeleteAsync(id);
            return Ok(new ApiResponse($"Клиент '{name}' успешно удалён"));
        }
        /// <summary>
        /// Отправить единицу измерения в архив
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var name = await _clientService.ArchiveAsync(id);
            return Ok(new ApiResponse($"Клиент '{name}' успешно архивирован"));
        }
        /// <summary>
        /// Убрать единицу измерения из архива
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/unarchive")]
        public async Task<IActionResult> Unarchive(int id)
        {
            var name = await _clientService.UnarchiveAsync(id);
            return Ok(new ApiResponse($"Клиент '{name}' успешно убрана из архива"));
        }
        /// <summary>
        /// Проверить имя на уникальность
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckNameUnique([FromQuery] string name)
        {
            bool isUnique = await _clientService.CheckNameUniqueAsync(name);
            return Ok(new ApiResponse<bool>(
                isUnique ? "Имя клиента свободно" : "Клиент с таким названием уже существует",
                isUnique
            ));
        }
    }
}
