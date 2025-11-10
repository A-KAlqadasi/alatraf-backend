
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Persons.Services;

public interface IPersonCreateService
{
    Task<Result<Person>> CreateAsync(string Fullname,
        DateTime Birthdate,
        string Phone,
        string? NationalNo,
        string Address, CancellationToken cancellationToken);

}
