using AlatrafClinic.Domain.Organization.DoctorSectionRooms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDoctorSectionRoomRepository : IGenericRepository<DoctorSectionRoom,int>
{
  Task<DoctorSectionRoom?> GetActiveAssignmentAsync(int doctorId, CancellationToken ct);
  Task<List<DoctorSectionRoom>> GetByDoctorIdAsync(int doctorId, CancellationToken ct);
}

