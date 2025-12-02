using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : Entity<TId>
{
    protected readonly AlatrafClinicDbContext dbContext;

    public GenericRepository(AlatrafClinicDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
    {
        var query = spec.Apply(dbContext.Set<TEntity>().AsQueryable());
        return await query.CountAsync(ct);
    }

    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, int page, int pageSize, CancellationToken ct = default)
    {
        var query = spec.Apply(dbContext.Set<TEntity>().AsQueryable());

        var skip = (page - 1) * pageSize;

        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await dbContext.Set<TEntity>().AddAsync(entity, ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Set<TEntity>().ToListAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await dbContext.Set<TEntity>().FindAsync(new object?[] { id }, ct);
    }

    public async Task<bool> IsExistAsync(TId id, CancellationToken ct = default)
    {
        return await dbContext.Set<TEntity>().AnyAsync(e => e.Id!.Equals(id), ct);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        dbContext.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }
}