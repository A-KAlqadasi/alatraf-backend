using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Departments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DepartmentRepository : GenericRepository<Department, int>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> IsExistAsync(string name, CancellationToken ct = default)
    {
       return await _dbContext.Departments.AnyAsync(d=> d.Name.Trim() == name.Trim());
    }
}