using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Appointments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class AppointmentRepository : GenericRepository<Appointment, int>, IAppointmentRepository
{

    public AppointmentRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int> GetAppointmentCountByDate(DateTime date, CancellationToken ct = default)
    {
        return await dbContext.Appointments
            .CountAsync(a => a.CreatedAtUtc.DateTime.Date == date.Date, ct);
    }

    public async Task<int> GetAppointmentCountByDateAndPatientType(DateTime date, PatientType patientType, CancellationToken ct = default)
    {
        return await dbContext.Appointments
            .Where(a => a.CreatedAtUtc.DateTime.Date == date.Date && a.PatientType == patientType)
            .CountAsync(ct);
    }

    public async Task<DateTime> GetLastAppointmentAttendDate(CancellationToken ct = default)
    {
        var lastAppointment = await dbContext.Appointments
            .OrderByDescending(a => a.AttendDate)
            .FirstOrDefaultAsync(ct);

        return lastAppointment?.AttendDate ?? DateTime.MinValue;
    }
}