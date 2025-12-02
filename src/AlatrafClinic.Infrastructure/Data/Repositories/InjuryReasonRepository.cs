using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class InjuryReasonRepository : GenericRepository<InjuryReason, int>, IInjuryReasonRepository
{
    public InjuryReasonRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}