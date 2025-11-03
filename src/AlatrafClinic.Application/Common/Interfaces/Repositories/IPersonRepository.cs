using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IPersonRepository : IGenericRepository<Person, int>
{
  Task<Person?> GetByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default);
  Task<Person?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
Task<bool> HasReferencesAsync(int personId, CancellationToken cancellationToken = default);


}
