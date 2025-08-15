using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Services
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly IDocumentShipmentRepository _documentShipmentRepository;

        public ClientService(ILogger<ClientService> logger,
            IClientRepository clientRepository,
            IDocumentShipmentRepository documentShipmentRepository)
        {
            _logger = logger;
            _clientRepository = clientRepository;
            _documentShipmentRepository = documentShipmentRepository;
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync()
        {
            var entities = await _clientRepository.GetAllAsync();
            return entities.Select(ClientMapper.ToDto);
        }

        public async Task<IEnumerable<ClientDto>> GetAllFilteredAsync(bool isActive = true)
        {
            var entities = await _clientRepository.GetAllFilteredAsync(isActive);
            return entities.Select(ClientMapper.ToDto);
        }
        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var entity = await _clientRepository.GetByIdWithTrackingAsync(id);
            return entity == null ? null : ClientMapper.ToDto(entity);
        }

        public async Task<ClientDto> CreateAsync(CreateClientDto dto)
        {
            if (await _clientRepository.AnyByNameAsync(dto.Name))
                throw new ConflictException("Клиент с таким названием уже существует");

            var entity = new Client
            {
                Name = dto.Name,
                Address = dto.Address,
                Status = (int)dto.Status
            };

            await _clientRepository.AddAsync(entity);
            await _clientRepository.SaveChangesAsync();

            return ClientMapper.ToDto(entity);
        }
        public async Task<string> UpdateAsync(int id, UpdateClientDto dto)
        {
            var entity = await _clientRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Клиент с Id={id} не найден.");

            if (dto.Name is not null &&
                !string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _clientRepository.AnyByNameAsync(dto.Name))
                    throw new ConflictException("Клиент с таким названием уже существует.");
            }

            ClientMapper.PatchEntity(entity, dto);

            _clientRepository.Update(entity);
            await _clientRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<string> DeleteAsync(int id)
        {
            var entity = await _clientRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Клиент с Id={id} не найден.");

            if (await IsInUseAsync(id))
                throw new ConflictException("Нельзя удалить клиента, если он используется. Переведите в архив.");

            await _clientRepository.Delete(id);

            return entity.Name;
        }

        //
        public async Task<string> ArchiveAsync(int id)
        {
            var entity = await _clientRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Клиент с Id={id} не найден.");

            entity.Status = (int)EntityStatus.Archived;
            _clientRepository.Update(entity);
            await _clientRepository.SaveChangesAsync();
            
            return entity.Name;
        }
        public async Task<string> UnarchiveAsync(int id)
        {
            var entity = await _clientRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Клиент с Id={id} не найден.");
            entity.Status = (int)EntityStatus.Active;
            _clientRepository.Update(entity);
            await _clientRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<bool> IsInUseAsync(int clientId)
        {
            var checkShipments = await _documentShipmentRepository.AnyByIdAsync(s => s.ClientId == clientId);

            return checkShipments;
        }

        public async Task<bool> CheckNameUniqueAsync(string name)
        {
            return !await _clientRepository.AnyByNameAsync(name);
        }
    }
}
