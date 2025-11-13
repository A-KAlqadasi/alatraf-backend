using AlatrafClinic.Domain.Organization.Departments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department, int>
{
    Task<Department?> GetByNameAsync(string name, CancellationToken ct);
    Task<List<Department>> SearchByNameAsync(string searchTerm, CancellationToken ct);
    Task<Department?> GetDepartmentByIdWithSectionsAndRoomsAsync(int id, CancellationToken ct);

}