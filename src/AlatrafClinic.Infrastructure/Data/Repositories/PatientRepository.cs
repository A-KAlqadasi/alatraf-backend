using AlatrafClinic.Domain.Patients;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class PatientRepository : GenericRepository<Patient, int>, IPatientRepository
{
    public PatientRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}