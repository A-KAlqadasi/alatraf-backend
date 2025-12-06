using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class UnitRepository : GenericRepository<Unit, int>, IUnitRepository
{
    public UnitRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}
