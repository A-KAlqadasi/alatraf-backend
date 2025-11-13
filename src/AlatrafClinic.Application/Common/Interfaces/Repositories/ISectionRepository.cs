using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ISectionRepository : IGenericRepository<Section, int>
{
  Task<List<Section>> GetAllSectionsFilteredAsync(int? departmentId, string? searchTerm, CancellationToken ct);
  Task<Section?> GetByNameInDepartmentAsync(string name, int departmentId, CancellationToken ct);
  Task AddRangeAsync(IEnumerable<Section> sections, CancellationToken ct);

  Task<Section?> GetSectionByIdWithDepartmentAsync(int id, CancellationToken ct);

}
