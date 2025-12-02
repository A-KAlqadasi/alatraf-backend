using AlatrafClinic.Domain.Departments.Sections;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ISectionRepository : IGenericRepository<Section, int>
{
    Task<bool> IsSectionHasRoomNameAsync(int sectionId, string roomName, CancellationToken ct);
    
}
