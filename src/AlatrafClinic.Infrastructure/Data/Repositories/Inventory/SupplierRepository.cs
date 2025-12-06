using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Domain.Inventory.Suppliers;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class SupplierRepository : GenericRepository<Supplier, int>, ISupplierRepository
{
    public SupplierRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}
