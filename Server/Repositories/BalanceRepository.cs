using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Server.Mappings;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Balance;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Repositories
{
    public class BalanceRepository : GenericRepository<Balance>, IBalanceRepository
    {
        private readonly ILogger<BalanceRepository> _logger;

        public BalanceRepository(AppDbContext dbContext, 
            ILogger<GenericRepository<Balance>> genericLogger, 
            ILogger<BalanceRepository> logger) 
            : base(dbContext, genericLogger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<VBalance>> GetAllViewAsync(BalanceFilterDto filter)
        {
            var query = _dbContext.VBalances.AsQueryable();

            if (filter.OnlyActive)
            {
                query = query.Where(b => b.ResourceStatus == (int)EntityStatus.Active &&
                                         b.MeasureStatus == (int)EntityStatus.Active);
            }

            if (filter.ResourceIds is not null && filter.ResourceIds.Any())
                query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId ?? 0));

            if (filter.MeasureIds is not null && filter.MeasureIds.Any())
                query = query.Where(b => filter.MeasureIds.Contains(b.MeasureId ?? 0));

            return await query.ToListAsync();
        }

        public async Task<VBalance?> GetViewByIdsAsync(int resourceId, int measureId)
        {
            return await _dbContext.VBalances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.MeasureId == measureId);
        }

        public async Task<Balance?> GetByIdsAsync(int resourceId, int measureId)
        {
            return await _dbContext.Balances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.MeasureId == measureId);
        }
        public async Task<bool> HasEnoughAsync(IEnumerable<ResourceQuantityDto> requiredResources)
        {
            foreach (var req in requiredResources)
            {
                var balance = await _dbContext.Balances
                    .FirstOrDefaultAsync(b => b.ResourceId == req.ResourceId && b.MeasureId == req.MeasureId);
                if (balance == null || balance.Quantity < req.Quantity)
                    return false;
            }
            return true;
        }

        public async Task IncreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToIncrease)
        {
            var grouped = resourcesToIncrease
                .GroupBy(r => new { r.ResourceId, r.MeasureId })
                .Select(g => new ResourceQuantityDto
                {
                    ResourceId = g.Key.ResourceId,
                    MeasureId = g.Key.MeasureId,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            var keys = grouped.Select(r => (r.ResourceId, r.MeasureId)).ToList();

            var predicate = BuildPredicate(keys);

            var existingBalances = await _dbContext.Balances
                .Where(predicate)
                .ToListAsync();

            foreach (var req in grouped)
            {
                var row = existingBalances.FirstOrDefault(b => b.ResourceId == req.ResourceId && b.MeasureId == req.MeasureId);
                if (row == null)
                {
                    await _dbContext.Balances.AddAsync(new Balance
                    {
                        ResourceId = req.ResourceId,
                        MeasureId = req.MeasureId,
                        Quantity = req.Quantity
                    });
                }
                else
                {
                    row.Quantity += req.Quantity;
                    _dbContext.Balances.Update(row);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DecreaseAsync(IEnumerable<ResourceQuantityDto> resourcesToDecrease)
        {
            var keys = resourcesToDecrease.Select(r => (r.ResourceId, r.MeasureId)).ToList();

            var predicate = BuildPredicate(keys);

            var existingBalances = await _dbContext.Balances
                .Where(predicate)
                .ToListAsync();

            foreach (var req in resourcesToDecrease)
            {
                var row = existingBalances.FirstOrDefault(b => b.ResourceId == req.ResourceId && b.MeasureId == req.MeasureId);
                if (row == null || row.Quantity < req.Quantity)
                    throw new ConflictException(
                        $"Недостаточно ресурса {req.ResourceId} (мера {req.MeasureId}) для списания");
                row.Quantity -= req.Quantity;
                _dbContext.Balances.Update(row);
            }

            await _dbContext.SaveChangesAsync();
        }

        private Expression<Func<Balance, bool>> BuildPredicate(List<(int ResourceId, int MeasureId)> keys)
        {
            var param = Expression.Parameter(typeof(Balance), "b");
            Expression predicate = Expression.Constant(false);

            foreach (var key in keys)
            {
                var resourceIdEquals = Expression.Equal(
                    Expression.Property(param, nameof(Balance.ResourceId)),
                    Expression.Constant(key.ResourceId));
                var measureIdEquals = Expression.Equal(
                    Expression.Property(param, nameof(Balance.MeasureId)),
                    Expression.Constant(key.MeasureId));
                var andCondition = Expression.AndAlso(resourceIdEquals, measureIdEquals);
                predicate = Expression.OrElse(predicate, andCondition);
            }

            return Expression.Lambda<Func<Balance, bool>>(predicate, param);
        }

    }
}
