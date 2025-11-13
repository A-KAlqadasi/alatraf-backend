
using AlatrafClinic.Domain.Organization.Rooms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IRoomRepository : IGenericRepository<Room, int>
{
  Task<List<Room>> GetAllRoomsFilteredAsync(int? sectionId, string? searchTerm, CancellationToken ct);
  Task AddRangeAsync(IEnumerable<Room> rooms, CancellationToken ct);
  Task<Room?> GetRoomByIdWithSectionAsync(int id, CancellationToken ct);

}