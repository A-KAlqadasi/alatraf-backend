using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class SessionRepository : GenericRepository<Session, int>, ISessionRepository
{
    public SessionRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}