using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> HasAssociationsAsync(TId id, CancellationToken ct = default);
}
