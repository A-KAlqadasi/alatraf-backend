using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Patients;

public interface IPatientRepository : IGenericRepository<Patient, int>
{
    Task<Patient?> GetByIdWithPersonAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IQueryable<Patient>> GetPatientsWithPersonQueryAsync(CancellationToken cancellationToken = default);
}