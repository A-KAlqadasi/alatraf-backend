

using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;

public interface IPersonUpdateService
{
    Task<Result<Person>> UpdateAsync(
        int personId,
        string fullname,
        DateTime birthdate,
        string phone,
        string? nationalNo,
        string address,
        CancellationToken ct);
}
