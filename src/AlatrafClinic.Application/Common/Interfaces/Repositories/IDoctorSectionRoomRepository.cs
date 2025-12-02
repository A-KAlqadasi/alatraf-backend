using AlatrafClinic.Domain.Departments.DoctorSectionRooms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDoctorSectionRoomRepository : IGenericRepository<DoctorSectionRoom,int>
{
    Task<DoctorSectionRoom?> GetActiveAssignmentByDoctorAndSectionIdsAsync(int doctorId, int sectionId, CancellationToken ct);

    Task<List<DoctorSectionRoom>> GetTechniciansActiveAssignmentsAsync(CancellationToken ct);
    Task<List<DoctorSectionRoom>> GetTherapistsActiveAssignmentsAsync(CancellationToken ct);
}

