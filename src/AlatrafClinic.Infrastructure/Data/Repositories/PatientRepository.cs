using AlatrafClinic.Domain.Patients;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class PatientRepository : GenericRepository<Patient, int>, IPatientRepository
{
    public PatientRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async new Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Set<Patient>().Include(p => p.Person)
                                            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}