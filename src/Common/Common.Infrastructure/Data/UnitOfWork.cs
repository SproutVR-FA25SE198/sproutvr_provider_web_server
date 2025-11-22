using System.Collections.Concurrent;
using Common.Application.Abstractions.Data;
using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Infrastructure.Data;

/// <summary>
/// Base implementation of the unit of work interface.
/// </summary>
public class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    /// <summary>
    /// Injects a DbContext instance to be used by all repositories.
    /// </summary>
    /// <param name="context"></param>
    public UnitOfWork(TDbContext context)
    {
        _dbContext = context;
    }

    /// <summary>
    /// The asynchronous Complete method persists changes made to the database.
    /// </summary>
    /// <returns>
    /// The task result contains the value true or false 
    /// based on the number of rows affected
    /// </returns>
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <summary>
    /// The Dispose method implements the IDisposable interface 
    /// and ensures proper resource cleanup associated with the UnitOfWork.
    /// </summary>
    public void Dispose()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// The Repository method provides a generic way to retrieve a 
    /// repository instance for a specific entity type T. 
    /// It implements a caching mechanism to optimize repository creation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// Casts the retrieved repository instance (from the cache or newly created) 
    /// to IGenericRepository of type T and returns it.
    /// </returns>
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        string type = typeof(T).Name;

        return (IGenericRepository<T>)_repositories.GetOrAdd(type, t =>
        {
            // Get the unbound generic type definition: GenericRepository<>
            Type genericRepoDefinition = typeof(Common.Infrastructure.Data.GenericRepository<>);

            // Call MakeGenericType on the definition, passing the desired entity type (T)
            Type repoType = genericRepoDefinition.MakeGenericType(typeof(T));

            // This will create an instance of that GenericRepository with DbContext injected
            return Activator.CreateInstance(repoType, _dbContext)
                ?? throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Could not create repository instance for {0}", t));
        });
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        await transaction.RollbackAsync(cancellationToken);
    }
}
