using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Patients.Enums;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class AppointmentRepository : GenericRepository<Appointment, int>, IAppointmentRepository
{

    public AppointmentRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int> GetAppointmentCountByDate(DateOnly date, CancellationToken ct = default)
    {
        return await dbContext.Appointments
            .CountAsync(a => DateOnly.FromDateTime(a.CreatedAtUtc.DateTime) == date, ct);
    }

    public async Task<int> GetAppointmentCountByDateAndPatientType(DateOnly date, PatientType patientType, CancellationToken ct = default)
    {
        return await dbContext.Appointments
            .Where(a => DateOnly.FromDateTime(a.CreatedAtUtc.DateTime) == date && a.PatientType == patientType)
            .CountAsync(ct);
    }

    public async Task<DateOnly> GetLastAppointmentAttendDate(CancellationToken ct = default)
    {
        var lastAppointment = await dbContext.Appointments
            .OrderByDescending(a => a.AttendDate)
            .FirstOrDefaultAsync(ct);

        return lastAppointment?.AttendDate ?? DateOnly.MinValue;
    }
}