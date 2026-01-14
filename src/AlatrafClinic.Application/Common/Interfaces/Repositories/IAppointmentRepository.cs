using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment, int>
{
    Task<DateOnly> GetLastAppointmentAttendDate(CancellationToken ct = default);
    Task<int> GetAppointmentCountByDate(DateOnly date, CancellationToken ct = default);
    Task<int> GetAppointmentCountByDateAndPatientType(DateOnly date, PatientType patientType, CancellationToken ct = default);
}