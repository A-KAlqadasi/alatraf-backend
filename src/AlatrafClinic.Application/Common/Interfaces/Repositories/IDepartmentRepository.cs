using AlatrafClinic.Domain.Departments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department, int>
{
    Task<bool> IsExistAsync(string name, CancellationToken ct = default);
}