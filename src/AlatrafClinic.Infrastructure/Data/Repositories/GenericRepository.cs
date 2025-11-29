
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : Entity<TId>
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().ToListAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object?[] { id }, ct);
    }

    public async Task<bool> IsExistAsync(TId id, CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.Id!.Equals(id), ct);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _context.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }
}