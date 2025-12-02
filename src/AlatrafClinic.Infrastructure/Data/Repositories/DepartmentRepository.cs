using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Departments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DepartmentRepository : GenericRepository<Department, int>, IDepartmentRepository
{
    public DepartmentRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> IsExistAsync(string name, CancellationToken ct = default)
    {
       return await dbContext.Departments.AnyAsync(d=> d.Name.Trim() == name.Trim());
    }
}