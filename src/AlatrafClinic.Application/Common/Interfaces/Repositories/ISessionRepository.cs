using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ISessionRepository : IGenericRepository<Session, int>
{
    Task<IQueryable<Session>> GetSessionsQueryAsync(CancellationToken ct = default);
}