using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.People.Employees;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee, Guid>
{
  Task<IReadOnlyList<Employee>> GetByRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<Employee?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
}
