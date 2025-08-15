using System.Xml;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Shared.Dtos.Balance;
using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly ILogger<ReceiptService> _logger;
        private readonly IReceiptResourceRepository _receiptResourceRepository;
        private readonly IDocumentReceiptRepository _documentReceiptRepository;
        private readonly IBalanceService _balanceService;

        public ReceiptService(ILogger<ReceiptService> logger, 
            IReceiptResourceRepository receiptResourceRepository, 
            IDocumentReceiptRepository documentReceiptRepository,
            IBalanceService balanceService)
        {
            _logger = logger;
            _receiptResourceRepository = receiptResourceRepository;
            _documentReceiptRepository = documentReceiptRepository;
            _balanceService = balanceService;
        }
        public async Task<IEnumerable<ReceiptWithResourcesDto>> GetAllAsync(ReceiptFilterDto filter)
        {
            var receipts = await _documentReceiptRepository.GetAllAsync(filter);
            var result = new List<ReceiptWithResourcesDto>();

            foreach (var receipt in receipts)
            {
                var resources = await _documentReceiptRepository.GetResourcesForDocumentAsync(receipt.Id);
                result.Add(ReceiptMapper.ReceiptDocumentWithResourceEntityToDto(receipt, resources));
            }
            return result;
        }

        public async Task<ReceiptWithResourcesDto?> GetByIdAsync(int id)
        {
            var receipt = await _documentReceiptRepository.GetByIdAsync(id);
            if (receipt == null)
                return null;

            var resources = await _documentReceiptRepository.GetResourcesForDocumentAsync(id);
            return ReceiptMapper.ReceiptDocumentWithResourceEntityToDto(receipt, resources);
        }
        public async Task<ReceiptWithResourcesDto> CreateAsync(CreateReceiptDto dto)
        {
            if (!await CheckNumberUniqueAsync(dto.DocumentNumber))
                throw new ConflictException($"Документ с номером '{dto.DocumentNumber}' уже существует");

            var receipt = new DocumentsReceipt
            {
                Number = dto.DocumentNumber,
                DateReceipt = dto.DateReceipt
            };

            await _documentReceiptRepository.AddAsync(receipt);
            await _documentReceiptRepository.SaveChangesAsync();

            if (dto.ReceiptResources != null && dto.ReceiptResources.Any())
            {
                if (dto.ReceiptResources != null && dto.ReceiptResources.Count > 0)
                {
                    foreach (var rr in dto.ReceiptResources)
                    {
                        await _receiptResourceRepository.AddAsync(new ReceiptResource
                        {
                            DocumentReceiptId = receipt.Id,
                            ResourceId = rr.ResourceId,
                            MeasureId = rr.MeasureId,
                            Quantity = rr.Quantity
                        });
                    }
                    await _receiptResourceRepository.SaveChangesAsync();

                    await _balanceService.IncreaseAsync(
                        dto.ReceiptResources.Select(rr => new ResourceQuantityDto
                        {
                            ResourceId = rr.ResourceId, 
                            MeasureId = rr.MeasureId,
                            Quantity = rr.Quantity
                        })
                    );
                }
            }
            return (await GetByIdAsync(receipt.Id))!;
        }
        public async Task<string> UpdateAsync(int id, UpdateReceiptDto dto)
        {
            var receipt = await _documentReceiptRepository.GetByIdAsync(id)
                           ?? throw new NotFoundException("Документ не найден");

            ICollection<ReceiptResource> resources = await _receiptResourceRepository.GetByDocumentIdAsync(id);
            receipt.ReceiptResources = resources;

            await _documentReceiptRepository.UpdateWithResourcesAsync(receipt, dto);

            return receipt.Number;
        }

        public async Task<string> DeleteAsync(int id)
        {
            var receipt = await _documentReceiptRepository.GetByIdAsync(id)
                                 ?? throw new NotFoundException("Документ не найден");

            var resources = await _receiptResourceRepository.GetByDocumentIdAsync(id);

            // Откатить баланс
            if (resources.Any())
            {
                var oldAsDto = resources.Select(o => new ResourceQuantityDto
                {
                    ResourceId = o.ResourceId,
                    MeasureId = o.MeasureId,
                    Quantity = o.Quantity
                }).ToList();
                await _balanceService.DecreaseAsync(oldAsDto);
            }

            await _receiptResourceRepository.DeleteByDocumentIdAsync(id);
            await _documentReceiptRepository.Delete(id);
            await _documentReceiptRepository.SaveChangesAsync();

            return receipt.Number;
        }

        public async Task<bool> CheckNumberUniqueAsync(string number) 
            => !await _documentReceiptRepository.ExistByNumberAsync(number);
        
    }
}
