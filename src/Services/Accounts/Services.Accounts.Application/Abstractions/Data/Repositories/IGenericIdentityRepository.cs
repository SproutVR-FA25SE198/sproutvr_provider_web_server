using System.Linq.Expressions;

namespace Services.Accounts.Application.Abstractions.Data.Repositories;
public interface IGenericIdentityRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<T?> GetByIdAsync(object id);

    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetWithIncludeAsync(params Expression<Func<T, object>>[] includes);

    Task<int> SaveChangesAsync();
}
