using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Diagnosises.InjurySides;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class InjurySideRepository : GenericRepository<InjurySide, int>, IInjurySideRepository
{
    public InjurySideRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}