using Microsoft.EntityFrameworkCore;
using Services.Catalogs.Domain.Entities;

namespace Common.Application.Abstractions.Data;

/// <summary>
/// Create a new repository with type T without creating a new class.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    bool Exists(int id);
    Task<bool> SaveAllAsync();
    void Attach(T t);
    EntityState GetEntityState(T entity);
}
