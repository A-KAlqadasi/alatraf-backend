

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MechanicShop.Application.Common.Errors;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;

public class PersonUpdateService : IPersonUpdateService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<PersonUpdateService> _logger;

  public PersonUpdateService(
      IUnitOfWork unitOfWork,
      ILogger<PersonUpdateService> logger)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
  }

  public async Task<Result<Person>> UpdateAsync(
      int personId,
      string fullname,
      DateTime birthdate,
      string phone,
      string? nationalNo,
      string address,
      CancellationToken ct)
  {
    var person = await _unitOfWork.Person.GetByIdAsync(personId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person {personId} not found for update.", personId);
      return ApplicationErrors.PersonNotFound;
    }

    if (!string.IsNullOrWhiteSpace(nationalNo))
    {
      var existing = await _unitOfWork.Person.GetByNationalNoAsync(nationalNo.Trim(), ct);

      if (existing is not null && existing.Id != personId)
      {
        _logger.LogWarning("National number already exists for another person: {NationalNo}", nationalNo);
        return PersonErrors.NationalNoExists;
      }
    }

    var updateResult = person.Update(
        fullname.Trim(),
        birthdate,
        phone.Trim(),
        nationalNo?.Trim(),
        address.Trim());

    if (updateResult.IsError)
    {
      _logger.LogWarning("Update failed for Person {PersonId}: {Error}", personId, updateResult.Errors);
      return updateResult.Errors;
    }

    _logger.LogInformation("Person domain entitynprepered to  updated  (not persisted yet).");

    return person;
  }
}