using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : Entity<TId>
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
    {
        var query = spec.Apply(_dbContext.Set<TEntity>().AsQueryable());
        return await query.CountAsync(ct);
    }

    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, int page, int pageSize, CancellationToken ct = default)
    {
        var query = spec.Apply(_dbContext.Set<TEntity>().AsQueryable());

        var skip = (page - 1) * pageSize;

        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Set<TEntity>().ToListAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await _dbContext.Set<TEntity>().FindAsync(new object?[] { id }, ct);
    }

    public async Task<bool> IsExistAsync(TId id, CancellationToken ct = default)
    {
        return await _dbContext.Set<TEntity>().AnyAsync(e => e.Id!.Equals(id), ct);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbContext.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }
}