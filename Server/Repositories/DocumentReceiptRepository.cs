using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Balance;
using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Repositories
{
    public class DocumentReceiptRepository : GenericRepository<DocumentsReceipt>, IDocumentReceiptRepository
    {
        private readonly ILogger<DocumentReceiptRepository> _logger;
        private readonly IBalanceRepository _balanceRepository;

        public DocumentReceiptRepository(AppDbContext dbContext,
            ILogger<GenericRepository<DocumentsReceipt>> genericLogger,
            ILogger<DocumentReceiptRepository> logger,
            IBalanceRepository balanceRepository) : base(dbContext, logger)
        {
            _logger = logger;
            _balanceRepository = balanceRepository;
        }

        public async Task<IEnumerable<DocumentsReceipt>> GetAllAsync(ReceiptFilterDto filter)
        {
            var query = _dbSet.AsQueryable();

            if (filter.DateFrom.HasValue)
                query = query.Where(d => d.DateReceipt >= filter.DateFrom.Value);
            if (filter.DateTo.HasValue)
                query = query.Where(d => d.DateReceipt <= filter.DateTo.Value);
            if (filter.DocumentNumbers != null && filter.DocumentNumbers.Any())
                query = query.Where(d => filter.DocumentNumbers.Contains(d.Number));
            if ((filter.ResourceIds != null && filter.ResourceIds.Any()) ||
               (filter.MeasureIds != null && filter.MeasureIds.Any()))
            {
                query = query.Where(d =>
                    _dbContext.ReceiptResources.Any(rr =>
                        rr.DocumentReceiptId == d.Id &&
                        (filter.ResourceIds == null || !filter.ResourceIds.Any() || filter.ResourceIds.Contains(rr.ResourceId)) &&
                        (filter.MeasureIds == null || !filter.MeasureIds.Any() || filter.MeasureIds.Contains(rr.MeasureId))
                    ));
            }

            return await query
                .OrderByDescending(d => d.DateReceipt)
                .ToListAsync();

        }
        public async Task<bool> ExistByNumberAsync(string number)
        {
            return await _dbSet.AnyAsync(d => d.Number == number);
        }

        public async Task<IEnumerable<string>> GetAllNumberAsync()
        {
            return await _dbSet
                .Select(d => d.Number)
                .OrderBy(n => n)
                .ToListAsync();
        }

        public async Task<IEnumerable<VReceiptDocumentResource>> GetResourcesForDocumentAsync(int documentReceiptId)
        {
            return await _dbContext.VReceiptDocumentResources.Where(v => v.DocumentReceiptId == documentReceiptId)
                .ToListAsync();
        }

        public async Task UpdateWithResourcesAsync(DocumentsReceipt entity, UpdateReceiptDto dto)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                entity.Number = dto.DocumentNumber;
                entity.DateReceipt = dto.DateReceipt;

                var oldResources = entity.ReceiptResources?.ToList() ?? new List<ReceiptResource>();

                if (dto.ReceiptResources == null || dto.ReceiptResources.Count == 0)
                {
                    if (oldResources.Any())
                    {
                        var decreaseList = oldResources.Select(r => new ResourceQuantityDto
                        {
                            ResourceId = r.ResourceId,
                            MeasureId = r.MeasureId,
                            Quantity = r.Quantity
                        }).ToList();

                        await _balanceRepository.DecreaseAsync(decreaseList);

                        entity.ReceiptResources?.Clear();
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }

                if (entity.ReceiptResources == null)
                    entity.ReceiptResources = new List<ReceiptResource>();

                var dtoIds = dto.ReceiptResources
                    .Where(r => r.Id != 0)
                    .Select(r => r.Id)
                    .ToHashSet();

                var toRemove = entity.ReceiptResources
                    .Where(er => !dtoIds.Contains(er.Id))
                    .ToList();

                if (toRemove.Any())
                {
                    var decreaseList = toRemove.Select(r => new ResourceQuantityDto
                    {
                        ResourceId = r.ResourceId,
                        MeasureId = r.MeasureId,
                        Quantity = r.Quantity
                    }).ToList();

                    await _balanceRepository.DecreaseAsync(decreaseList);

                    foreach (var rem in toRemove)
                    {
                        entity.ReceiptResources.Remove(rem);
                        _dbContext.Set<ReceiptResource>().Remove(rem);
                    }
                }

                foreach (var rDto in dto.ReceiptResources)
                {
                    if (rDto.Id != 0)
                    {
                        var existing = entity.ReceiptResources.FirstOrDefault(er => er.Id == rDto.Id);
                        if (existing != null)
                        {
                            int diffQuantity = rDto.Quantity - existing.Quantity;

                            if (diffQuantity > 0)
                            {
                                await _balanceRepository.IncreaseAsync(new[] {
                                new ResourceQuantityDto { ResourceId = existing.ResourceId, MeasureId = existing.MeasureId, Quantity = diffQuantity }
                            });
                            }
                            else if (diffQuantity < 0)
                            {
                                await _balanceRepository.DecreaseAsync(new[] {
                                new ResourceQuantityDto { ResourceId = existing.ResourceId, MeasureId = existing.MeasureId, Quantity = -diffQuantity }
                            });
                            }

                            existing.ResourceId = rDto.ResourceId;
                            existing.MeasureId = rDto.MeasureId;
                            existing.Quantity = rDto.Quantity;
                        }
                        else
                        {
                            entity.ReceiptResources.Add(new ReceiptResource
                            {
                                Id = rDto.Id,
                                ResourceId = rDto.ResourceId,
                                MeasureId = rDto.MeasureId,
                                Quantity = rDto.Quantity,
                                DocumentReceiptId = entity.Id
                            });
                            await _balanceRepository.IncreaseAsync(new[] {
                            new ResourceQuantityDto { ResourceId = rDto.ResourceId, MeasureId = rDto.MeasureId, Quantity = rDto.Quantity }
                        });
                        }
                    }
                    else
                    {
                        entity.ReceiptResources.Add(new ReceiptResource
                        {
                            ResourceId = rDto.ResourceId,
                            MeasureId = rDto.MeasureId,
                            Quantity = rDto.Quantity,
                            DocumentReceiptId = entity.Id
                        });
                        await _balanceRepository.IncreaseAsync(new[] {
                        new ResourceQuantityDto { ResourceId = rDto.ResourceId, MeasureId = rDto.MeasureId, Quantity = rDto.Quantity }
                    });
                    }
                }

                _dbSet.Update(entity);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
