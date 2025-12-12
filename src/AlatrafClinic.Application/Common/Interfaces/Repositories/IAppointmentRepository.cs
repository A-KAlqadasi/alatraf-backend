using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Appointments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment, int>
{
    Task<DateOnly> GetLastAppointmentAttendDate(CancellationToken ct = default);
    Task<int> GetAppointmentCountByDate(DateOnly date, CancellationToken ct = default);
    Task<int> GetAppointmentCountByDateAndPatientType(DateOnly date, PatientType patientType, CancellationToken ct = default);
}