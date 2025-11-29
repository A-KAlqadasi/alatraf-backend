using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TherapyCardRepository : GenericRepository<TherapyCard, int>, ITherapyCardRepository
{
    public TherapyCardRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}