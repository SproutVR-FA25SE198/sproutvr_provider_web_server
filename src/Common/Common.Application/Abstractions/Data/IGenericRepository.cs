using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Abstractions.Data;

/// <summary>
/// Create a new repository with type T without creating a new class.
/// </summary>
/// <typeparam name="T"></typeparam>

#pragma warning disable S2326 // 'TDbContext' is not used in the interface.
public interface IGenericRepository<T, TDbContext>
    where T : BaseEntity
    where TDbContext : DbContext
{
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    bool Exists(Guid id);
    Task<bool> SaveAllAsync();
    void Attach(T t);
    EntityState GetEntityState(T entity);
}
#pragma warning restore S236
