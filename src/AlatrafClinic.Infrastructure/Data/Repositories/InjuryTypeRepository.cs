using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class InjuryTypeRepository : GenericRepository<InjuryType, int>, IInjuryTypeRepository
{
    public InjuryTypeRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}