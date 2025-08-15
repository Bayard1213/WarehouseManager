using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Models;

namespace WarehouseManager.Server.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(AppDbContext dbContext, ILogger<GenericRepository<T>> logger)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();

            _logger = logger;
        }

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public virtual async Task Delete(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"Сущность с id={id} не найдена.");

                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                _logger.Log(LogLevel.Error,ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);


        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();

        public void Update(T entity) => _dbSet.Update(entity);

        public async Task<bool> AnyByIdAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
