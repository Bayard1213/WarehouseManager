using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Shared.Dtos.Balance;
using WarehouseManager.Shared.Dtos.Shipment;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly ILogger<ShipmentService> _logger;
        private readonly IDocumentShipmentRepository _documentShipmentRepository;
        private readonly IShipmentResourceRepository _shipmentResourceRepository;
        private readonly IBalanceService _balanceService;

        public ShipmentService(ILogger<ShipmentService> logger, IDocumentShipmentRepository documentShipmentReapository, IShipmentResourceRepository shipmentResourceReapository, IBalanceService balanceService)
        {
            _logger = logger;
            _documentShipmentRepository = documentShipmentReapository;
            _shipmentResourceRepository = shipmentResourceReapository;
            _balanceService = balanceService;
        }

        public async Task<IEnumerable<ShipmentWithResourcesDto>> GetAllAsync(ShipmentFilterDto filter)
        {
            var shipments = await _documentShipmentRepository.GetAllAsync(filter);
            var result = new List<ShipmentWithResourcesDto>();

            foreach (var shipment in shipments)
            {
                var resources = await _documentShipmentRepository.GetResourcesForDocumentAsync(shipment.Id);
                result.Add(ShipmentMapper.ShipmentDocumentWithResourceEntityToDto(shipment, resources));
            }

            return result;
        }
        public async Task<ShipmentWithResourcesDto?> GetByIdAsync(int id)
        {
            var shipment = await _documentShipmentRepository.GetByIdWithClientAsync(id);
            if (shipment == null)
                return null;

            var resources = await _documentShipmentRepository.GetResourcesForDocumentAsync(id);
            return ShipmentMapper.ShipmentDocumentWithResourceEntityToDto(shipment, resources);
        }
        public async Task<ShipmentWithResourcesDto> CreateAsync(CreateShipmentDto dto)
        {
            if (!await CheckNumberUniqueAsync(dto.DocumentNumber))
                throw new ConflictException($"Документ с номером '{dto.DocumentNumber}' уже существует");

            if (dto.ShipmentResources == null || !dto.ShipmentResources.Any())
                throw new ValidationException("Отгрузка не может быть пустой");

            var shipment = new DocumentsShipment
            {
                Number = dto.DocumentNumber,
                ClientId = dto.ClientId,
                DateShipment = dto.DateShipment,
                Status = (int)DocumentStatus.Draft, 
            };

            await _documentShipmentRepository.AddAsync(shipment);
            await _documentShipmentRepository.SaveChangesAsync();

            foreach (var r in dto.ShipmentResources)
            {
                await _shipmentResourceRepository.AddAsync(new ShipmentResource
                {
                    DocumentShipmentId = shipment.Id,
                    ResourceId = r.ResourceId,
                    MeasureId = r.MeasureId,
                    Quantity = r.Quantity
                });
            }
            await _shipmentResourceRepository.SaveChangesAsync();

            return (await GetByIdAsync(shipment.Id))!;
        }
        public async Task<string> UpdateAsync(int id, UpdateShipmentDto dto)
        {
            var shipment = await _documentShipmentRepository.GetByIdAsync(id)
             ?? throw new NotFoundException("Документ не найден");

            if (IsSigned(shipment))
                throw new ConflictException("Редактировать подписанную отгрузку нельзя");

            ICollection<ShipmentResource> resources = await _shipmentResourceRepository.GetByDocumentIdAsync(id);
            shipment.ShipmentResources = resources;

            await _documentShipmentRepository.UpdateWithResourcesAsync(shipment, dto);

            return shipment.Number;
        }
        public async Task<string> DeleteAsync(int id)
        {
            var shipment = await _documentShipmentRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException("Документ не найден");

            if (IsSigned(shipment))
                throw new ConflictException("Удалить подписанную отгрузку нельзя — сначала отзовите её ");

            await _shipmentResourceRepository.DeleteByDocumentIdAsync(id);
            await _documentShipmentRepository.Delete(id);
            await _documentShipmentRepository.SaveChangesAsync();

            return shipment.Number;
        }

        //
        public Task<IEnumerable<string>> GetDocumentNumbersAsync()
            => _documentShipmentRepository.GetAllNumbersAsync();
        public async Task<bool> CheckNumberUniqueAsync(string number)
            => !await _documentShipmentRepository.ExistsByNumberAsync(number);

        public async Task<string> SignAsync(int id)
        {
            var shipment = await _documentShipmentRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException("Документ не найден");

            if (IsSigned(shipment))
                throw new ConflictException("Отгрузка уже подписана");

            var resources = await _shipmentResourceRepository.GetByDocumentIdAsync(shipment.Id);
            if (!await _balanceService.HasEnoughAsync(resources.Select(r => new ResourceQuantityDto
            {
                ResourceId = r.ResourceId,
                MeasureId = r.MeasureId,
                Quantity = r.Quantity
            })))
            {
                throw new ConflictException("Недостаточно ресурсов для подписания отгрузки");
            }

            await _balanceService.DecreaseAsync(resources.Select(r => new ResourceQuantityDto
            {
                ResourceId = r.ResourceId,
                MeasureId = r.MeasureId,
                Quantity = r.Quantity
            }));

            shipment.Status = (int)DocumentStatus.Signed;
            _documentShipmentRepository.Update(shipment);
            await _documentShipmentRepository.SaveChangesAsync();

            return shipment.Number;
        }
        public async Task<string> RevokeAsync(int id)
        {
            var shipment = await _documentShipmentRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException("Документ не найден");

            if (!IsSigned(shipment))
                throw new ConflictException("Отгрузка не подписана");

            var resources = await _shipmentResourceRepository.GetByDocumentIdAsync(shipment.Id);

            // Возвращаем на баланс
            await _balanceService.IncreaseAsync(resources.Select(r => new ResourceQuantityDto
            {
                ResourceId = r.ResourceId,
                MeasureId = r.MeasureId,
                Quantity = r.Quantity
            }));

            shipment.Status = (int)DocumentStatus.Revoked;
            _documentShipmentRepository.Update(shipment);
            await _documentShipmentRepository.SaveChangesAsync();

            return shipment.Number;
        }

        private bool IsSigned(DocumentsShipment shipment)
            => shipment.Status == (int)DocumentStatus.Signed;
    }
}
