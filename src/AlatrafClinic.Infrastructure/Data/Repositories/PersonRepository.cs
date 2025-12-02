using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.People;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class PersonRepository : GenericRepository<Person, int>, IPersonRepository
{
    public PersonRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Person?> GetByNationalNoAsync(string nationalNo, CancellationToken ct = default)
    {
        return await dbContext.People.FirstOrDefaultAsync(p => p.NationalNo == nationalNo, ct);
    }

    public async Task<bool> IsNationalNumberExistAsync(string nationalNo, CancellationToken ct = default)
    {
        return await dbContext.People.AnyAsync(p => p.NationalNo == nationalNo.Trim(), ct);
    }

    public async Task<bool> IsPhoneNumberExistAsync(string phoneNo, CancellationToken ct = default)
    {
        return await dbContext.People.AnyAsync(p => p.Phone.Trim() == phoneNo.Trim(), ct);
    }

    public async Task<bool> IsNameExistAsync(string fullName, CancellationToken ct = default)
    {
        return await dbContext.People.AnyAsync(p => p.FullName.Trim() == fullName.Trim(), ct);
    }
}