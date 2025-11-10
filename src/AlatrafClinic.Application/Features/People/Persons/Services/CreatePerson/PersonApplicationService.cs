
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Services;

public class PersonCreateService : IPersonCreateService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<PersonCreateService> _logger;

  public PersonCreateService(
      IUnitOfWork unitOfWork,
      ILogger<PersonCreateService> logger)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
  }

  public async Task<Result<Person>> CreateAsync(string Fullname,
        DateTime Birthdate,
        string Phone,
        string? NationalNo,
        string Address, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(NationalNo))
    {
      var existing = await _unitOfWork.Person
          .GetByNationalNoAsync(NationalNo.Trim(), cancellationToken);

      if (existing is not null)
      {
        _logger.LogWarning("Person creation aborted. National number already exists: {NationalNo}", NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }

    var createResult = Person.Create(
        Fullname.Trim(),
             Birthdate,
             Phone.Trim(),
             NationalNo?.Trim(),
             Address.Trim());

    if (createResult.IsError)
      return createResult.Errors;
      

        _logger.LogInformation("Person domain entity  prepered to created (not persisted yet).");


    return createResult.Value;
  }
}
