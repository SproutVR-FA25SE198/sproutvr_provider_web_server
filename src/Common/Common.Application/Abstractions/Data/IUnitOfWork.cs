using Common.Domain.Entities;

namespace Common.Application.Abstractions.Data;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
}
