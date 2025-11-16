using AlatrafClinic.Domain.Organization.DoctorSectionRooms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDoctorSectionRoomRepository : IGenericRepository<DoctorSectionRoom,int>
{
    Task<bool> HasActiveAssignmentByRoomIdAsync(int roomId, CancellationToken ct);
    Task<DoctorSectionRoom?> GetActiveAssignmentAsync(int doctorId, CancellationToken ct);
    Task<List<DoctorSectionRoom>> GetByDoctorIdAsync(int doctorId, CancellationToken ct);
    Task<DoctorSectionRoom?> GetActiveAssignmentByDoctorAndSectionIdsAsync(int doctorId, int sectionId, CancellationToken ct);

    Task<List<DoctorSectionRoom>> GetTechniciansActiveAssignmentsAsync(CancellationToken ct);
    Task<List<DoctorSectionRoom>> GetTherapistsActiveAssignmentsAsync(CancellationToken ct);
}

