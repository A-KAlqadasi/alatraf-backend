using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Services.UpdatePerson;

public interface IPersonUpdateService
{
    Task<Result<Person>> UpdateAsync(
        int personId,
        string Fullname,
        DateTime Birthdate,
        string Phone,
        string? NationalNo,
        string Address,
        bool Gender,
        CancellationToken ct);
}
