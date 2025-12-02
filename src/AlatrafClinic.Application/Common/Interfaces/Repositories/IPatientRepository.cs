using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Patients;

public interface IPatientRepository : IGenericRepository<Patient, int>
{
}