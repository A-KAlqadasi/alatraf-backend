using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DoctorSectionRoomRepository : GenericRepository<DoctorSectionRoom, int>, IDoctorSectionRoomRepository
{
    public DoctorSectionRoomRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DoctorSectionRoom?> GetActiveAssignmentByDoctorAndSectionIdsAsync(int doctorId, int sectionId, CancellationToken ct)
    {
        return await dbContext.DoctorSectionRooms
            .FirstOrDefaultAsync(dsrm => dsrm.DoctorId == doctorId
                                        && dsrm.SectionId == sectionId
                                        && dsrm.IsActive, ct);
    }

    public async Task<List<DoctorSectionRoom>> GetTechniciansActiveAssignmentsAsync(CancellationToken ct)
    {
        return await dbContext.DoctorSectionRooms
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodayIndustrialPartsCount() > 0)
            .ToListAsync(ct);
    }

    public async Task<List<DoctorSectionRoom>> GetTherapistsActiveAssignmentsAsync(CancellationToken ct)
    {
        return await dbContext.DoctorSectionRooms
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodaySessionsCount() > 0)
            .ToListAsync(ct);
    }
}