using System.Linq.Expressions;
using Common.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Infrastructure.Data.Database;

namespace Services.Accounts.Infrastructure.Data.Repositories;
public class GenericIdentityRepository<T> : IGenericIdentityRepository<T> where T : class
{
    protected readonly AccountDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericIdentityRepository(AccountDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetWithIncludeAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        foreach (Expression<Func<T, object>> include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        IQueryable<T> query = _dbSet.AsQueryable();
        query = spec.ApplyCriteria(query);
        return await query.CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }

        if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = spec.ThenIncludes.Aggregate(query, (current, include) => include(current));

        return query;
    }
}
