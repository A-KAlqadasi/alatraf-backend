using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Services.CreatePerson;

public interface IPersonCreateService
{
    Task<Result<Person>> CreateAsync(
        string Fullname,
        DateOnly Birthdate,
        string Phone,
        string? NationalNo,
        int AddressId,
        bool Gender, CancellationToken cancellationToken);
}
