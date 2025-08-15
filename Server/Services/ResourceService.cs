using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Shared.Dtos.Resource;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ILogger<ResourceService> _logger;
        private readonly IResourceRepository _resourceRepository;
        private readonly IReceiptResourceRepository _receiptResourceRepository;
        private readonly IShipmentResourceRepository _shipmentResourceRepository;
        private readonly IBalanceRepository _balanceRepository;

        public ResourceService(ILogger<ResourceService> logger,
            IResourceRepository resourceRepository,
            IReceiptResourceRepository receiptResourceRepository,
            IShipmentResourceRepository shipmentResourceRepository,
            IBalanceRepository balanceRepository)
        {
            _logger = logger;
            _resourceRepository = resourceRepository;
            _receiptResourceRepository = receiptResourceRepository;
            _shipmentResourceRepository = shipmentResourceRepository;
            _balanceRepository = balanceRepository;
        }

        public async Task<IEnumerable<ResourceDto>> GetAllAsync()
        {
            var entities = await _resourceRepository.GetAllAsync();
            return entities.Select(ResourceMapper.ToDto);
        }

        public async Task<IEnumerable<ResourceDto>> GetAllFilteredAsync(bool isActive = true)
        {
            var entities = await _resourceRepository.GetAllFilteredAsync(isActive);
            return entities.Select(ResourceMapper.ToDto);
        }
        public async Task<ResourceDto?> GetByIdAsync(int id)
        {
            var entity = await _resourceRepository.GetByIdWithTrackingAsync(id);
            return entity == null ? null : ResourceMapper.ToDto(entity);
        }
        public async Task<ResourceDto> CreateAsync(CreateResourceDto dto)
        {
            if (await _resourceRepository.AnyByNameAsync(dto.Name))
                throw new ConflictException("Ресурс с таким названием уже существует");

            var entity = new Resource
            {
                Name = dto.Name,
                Status = (int)dto.Status
            };

            await _resourceRepository.AddAsync(entity);
            await _resourceRepository.SaveChangesAsync();

            return ResourceMapper.ToDto(entity);
        }
        public async Task<string> UpdateAsync(int id, UpdateResourceDto dto)
        {
            var entity = await _resourceRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Ресурс с Id={id} не найден.");

            if (dto.Name != null && !string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _resourceRepository.AnyByNameAsync(dto.Name))
                    throw new ConflictException("Ресурс с таким названием уже существует.");
            }

            ResourceMapper.PatchEntity(entity, dto);

            _resourceRepository.Update(entity);
            await _resourceRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<string> DeleteAsync(int id)
        {
            var entity = await _resourceRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Ресурс с Id={id} не найден.");

            if (await IsInUseAsync(id))
                throw new ConflictException("Нельзя удалить ресурс, если он используется. Переведите в архив.");

            await _resourceRepository.Delete(id);

            return entity.Name;
        }
        //
        public async Task<string> ArchiveAsync(int id)
        {
            var entity = await _resourceRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Ресурс с Id={id} не найден.");

            entity.Status = (int)EntityStatus.Archived;
            _resourceRepository.Update(entity);
            await _resourceRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<string> UnarchiveAsync(int id)
        {
            var entity = await _resourceRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Ресурс с Id={id} не найден.");

            entity.Status = (int)EntityStatus.Active;
            _resourceRepository.Update(entity);
            await _resourceRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<bool> CheckNameUniqueAsync(string name)
        {
            return !await _resourceRepository.AnyByNameAsync(name);
        }

        public async Task<bool> IsInUseAsync(int resourceId)
        {
            var checkBalances = await _balanceRepository.AnyByIdAsync(m => m.MeasureId == resourceId);
            var checkReceipts = await _receiptResourceRepository.AnyByIdAsync(m => m.MeasureId == resourceId);
            var checkShipments = await _shipmentResourceRepository.AnyByIdAsync(m => m.MeasureId == resourceId);

            return checkBalances || checkReceipts || checkShipments;
        }


    }
}
