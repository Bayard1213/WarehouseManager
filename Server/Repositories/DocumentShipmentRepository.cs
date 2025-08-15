using Microsoft.EntityFrameworkCore;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Receipt;
using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Server.Repositories
{
    public class DocumentShipmentRepository : GenericRepository<DocumentsShipment>, IDocumentShipmentRepository
    {
        private readonly ILogger<DocumentShipmentRepository> _logger;

        public DocumentShipmentRepository(AppDbContext dbContext,
            ILogger<GenericRepository<DocumentsShipment>> genericLogger,
            ILogger<DocumentShipmentRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<DocumentsShipment>> GetAllAsync(ShipmentFilterDto filter)
        {
            var query = _dbSet.Include(d => d.Client).AsQueryable();

            if (filter.DateFrom.HasValue)
                query = query.Where(d => d.DateShipment >= filter.DateFrom.Value);
            if (filter.DateTo.HasValue)
                query = query.Where(d => d.DateShipment <= filter.DateTo.Value);
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
                .OrderByDescending(d => d.DateShipment)
                .ToListAsync();
        }

        public async Task<IEnumerable<VShipmentDocumentResource>> GetResourcesForDocumentAsync(int documentShipmentId)
        {
            return await _dbContext.VShipmentDocumentResources
                       .Where(v => v.DocumentShipmentId == documentShipmentId)
                       .ToListAsync();
        }
        public async Task<bool> ExistsByNumberAsync(string number)
        {
            return await _dbSet.AnyAsync(d => d.Number == number);
        }
        public async Task<IEnumerable<string>> GetAllNumbersAsync()
        {
            return await _dbSet
                      .Select(d => d.Number)
                      .OrderBy(n => n)
                      .ToListAsync();
        }
        public async Task UpdateWithResourcesAsync(DocumentsShipment entity, UpdateShipmentDto dto)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                entity.Number = dto.DocumentNumber;
                entity.DateShipment = dto.DateShipment;
                entity.Status = (int)dto.Status;

                if (dto.ShipmentResources == null || dto.ShipmentResources.Count == 0)
                {
                    if (entity.ShipmentResources != null)
                        entity.ShipmentResources.Clear();

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }

                if (entity.ShipmentResources == null)
                    entity.ShipmentResources = new List<ShipmentResource>();

                var dtoIds = dto.ShipmentResources
                    .Where(r => r.Id != 0)
                    .Select(r => r.Id)
                    .ToHashSet();

                var toRemove = entity.ShipmentResources
                    .Where(er => !dtoIds.Contains(er.Id))
                    .ToList();

                foreach (var rem in toRemove)
                {
                    entity.ShipmentResources.Remove(rem);
                    _dbContext.Set<ShipmentResource>().Remove(rem);
                }

                foreach (var rDto in dto.ShipmentResources)
                {
                    if (rDto.Id != 0)
                    {
                        var existing = entity.ShipmentResources.FirstOrDefault(er => er.Id == rDto.Id);
                        if (existing != null)
                        {
                            existing.ResourceId = rDto.ResourceId;
                            existing.MeasureId = rDto.MeasureId;
                            existing.Quantity = rDto.Quantity;
                        }
                        else
                        {
                            entity.ShipmentResources.Add(new ShipmentResource
                            {
                                Id = rDto.Id,
                                ResourceId = rDto.ResourceId,
                                MeasureId = rDto.MeasureId,
                                Quantity = rDto.Quantity,
                                DocumentShipmentId = entity.Id
                            });
                        }
                    }
                    else
                    {
                        entity.ShipmentResources.Add(new ShipmentResource
                        {
                            ResourceId = rDto.ResourceId,
                            MeasureId = rDto.MeasureId,
                            Quantity = rDto.Quantity,
                            DocumentShipmentId = entity.Id
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

        public async Task<DocumentsShipment?> GetByIdWithClientAsync(int id)
        {
            return await _dbContext.DocumentsShipments
                .Include(s => s.Client)               
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
