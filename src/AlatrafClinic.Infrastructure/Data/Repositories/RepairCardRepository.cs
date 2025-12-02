using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.RepairCards;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class RepairCardRepository : GenericRepository<RepairCard, int>, IRepairCardRepository
{
    public RepairCardRepository(AlatrafClinicDbContext context) : base(context)
    {
        
    }
}