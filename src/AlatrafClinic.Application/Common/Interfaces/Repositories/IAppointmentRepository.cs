using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Appointments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment, int>
{
    Task<DateTime> GetLastAppointmentDate(CancellationToken ct = default);
    Task<int> GetAppointmentCountByDate(DateTime date, CancellationToken ct = default);
    Task<int> GetAppointmentCountByDateAndPatientType(DateTime date, PatientType patientType, CancellationToken ct = default);
    Task<IQueryable<Appointment>> GetAppointmentsQueryAsync(CancellationToken ct = default);
}