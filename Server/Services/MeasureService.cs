using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Shared.Dtos.Measure;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Services
{
    public class MeasureService : IMeasureService
    {
        private readonly ILogger<MeasureService> _logger;
        private readonly IMeasureRepository _measureRepository;
        private readonly IReceiptResourceRepository _receiptResourceRepository; 
        private readonly IShipmentResourceRepository _shipmentResourceRepository;
        private readonly IBalanceRepository _balanceRepository;

        public MeasureService(ILogger<MeasureService> logger, 
            IMeasureRepository measureRepository, 
            IReceiptResourceRepository receiptResourceRepository,
            IShipmentResourceRepository shipmentResourceRepository,
            IBalanceRepository balanceRepository)
        {
            _logger = logger;
            _measureRepository = measureRepository;
            _receiptResourceRepository = receiptResourceRepository;
            _shipmentResourceRepository = shipmentResourceRepository;
            _balanceRepository = balanceRepository;
        }
        public async Task<IEnumerable<MeasureDto>> GetAllAsync()
        {
            var entities = await _measureRepository.GetAllAsync();
            return entities.Select(MeasureMapper.ToDto);
        }
        public async Task<IEnumerable<MeasureDto>> GetAllFilteredAsync(bool isActive = true)
        {
            var entities = await _measureRepository.GetAllFilteredAsync(isActive);
            return entities.Select(MeasureMapper.ToDto);
        }
        public async Task<MeasureDto?> GetByIdAsync(int id)
        {
            var entity = await _measureRepository.GetByIdWithTrackingAsync(id);
            return entity == null ? null : MeasureMapper.ToDto(entity);
        }
        public async Task<MeasureDto> CreateAsync(CreateMeasureDto dto)
        {
            if (await _measureRepository.AnyByNameAsync(dto.Name))
                throw new ConflictException("Единица измерения с таким названием уже существует");

            var entity = new Measure
            {
                Name = dto.Name,
                Status = (int)dto.Status
            };

            await _measureRepository.AddAsync(entity);
            await _measureRepository.SaveChangesAsync();

            return MeasureMapper.ToDto(entity);
        }
        public async Task<string> UpdateAsync(int id, UpdateMeasureDto dto)
        {
            var entity = await _measureRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Единица измерения с Id={id} не найдена.");

            if (dto.Name is not null &&
                !string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _measureRepository.AnyByNameAsync(dto.Name))
                    throw new ConflictException("Единица измерения с таким названием уже существует.");
            }

            MeasureMapper.PatchEntity(entity, dto);

            _measureRepository.Update(entity);
            await _measureRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<string> DeleteAsync(int id)
        {
            var entity = await _measureRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Единица измерения с Id={id} не найдена.");

            if (await IsInUseAsync(id))
                throw new ConflictException("Нельзя удалить единицу измерения, если она используется. Переведите в архив.");

            await _measureRepository.Delete(id);
            
            return entity.Name;
        }

        //
        public async Task<string> ArchiveAsync(int id)
        {
            var entity = await _measureRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Единица измерения с Id={id} не найдена.");

            entity.Status = (int)EntityStatus.Archived;
            _measureRepository.Update(entity);
            await _measureRepository.SaveChangesAsync();

            return entity.Name;
        }

        public async Task<string> UnarchiveAsync(int id)
        {
            var entity = await _measureRepository.GetByIdWithTrackingAsync(id);
            if (entity == null)
                throw new NotFoundException($"Единица измерения с Id={id} не найдена.");
            entity.Status = (int)EntityStatus.Active;
            _measureRepository.Update(entity);
            await _measureRepository.SaveChangesAsync();

            return entity.Name;
        }
        public async Task<bool> CheckNameUniqueAsync(string name)
        {
            return !await _measureRepository.AnyByNameAsync(name);
        }

        public async Task<bool> IsInUseAsync(int measureId)
        {
            var checkBalances = await  _balanceRepository.AnyByIdAsync(m => m.MeasureId == measureId);
            var checkReceipts = await _receiptResourceRepository.AnyByIdAsync(m => m.MeasureId == measureId);
            var checkShipments = await _shipmentResourceRepository.AnyByIdAsync(m => m.MeasureId == measureId);

            return checkBalances || checkReceipts || checkShipments;
        }
    }
}
