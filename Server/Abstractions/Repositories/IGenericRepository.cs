using System.Linq.Expressions;

namespace WarehouseManager.Server.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        Task Delete(int id);
        Task SaveChangesAsync();
        Task<bool> AnyByIdAsync(Expression<Func<T, bool>> predicate);


    }
}
