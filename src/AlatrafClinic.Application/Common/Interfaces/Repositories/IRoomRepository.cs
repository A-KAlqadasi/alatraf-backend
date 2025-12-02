
using AlatrafClinic.Domain.Departments.Sections.Rooms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IRoomRepository : IGenericRepository<Room, int>
{
  Task<List<Room>> GetAllRoomsFilteredAsync(int? sectionId, bool? isActiveDoctor, string? searchTerm, CancellationToken ct);
}