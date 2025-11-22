using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Application.Abstractions.Data;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
    //Transaction Methods
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
}
