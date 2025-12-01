using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IPersonRepository : IGenericRepository<Person, int>
{
    Task<Person?> GetByNationalNoAsync(string nationalNo, CancellationToken ct = default);
    Task<Person?> GetByPhoneAsync(string phone, CancellationToken ct = default);
    Task<bool> HasReferencesAsync(int personId, CancellationToken ct = default);
    Task<bool> IsNationalNumberExistAsync(string nationalNo, CancellationToken ct = default);
    Task<bool> IsPhoneNumberExistAsync(string phoneNo, CancellationToken ct = default);
    Task<bool> IsNameExistAsync(string fullName, CancellationToken ct = default);
}
