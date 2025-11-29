using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
    Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> IsExistAsync(TId id, CancellationToken ct = default);
}
