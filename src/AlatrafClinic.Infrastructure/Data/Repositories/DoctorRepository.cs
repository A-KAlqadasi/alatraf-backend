using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.People.Doctors;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DoctorRepository : GenericRepository<Doctor, int>, IDoctorRepository
{
    public DoctorRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}