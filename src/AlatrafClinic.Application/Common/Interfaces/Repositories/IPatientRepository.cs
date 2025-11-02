using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Enums;

public interface IPatientRepository : IGenericRepository<Patient, int>
{
  Task<Patient?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<Patient>> GetByPatientTypeAsync(PatientType type, CancellationToken cancellationToken = default);
}