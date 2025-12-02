using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;
using MechanicShop.Application.Common.Errors;

namespace AlatrafClinic.Application.Features.People.Services.UpdatePerson;

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
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string? NationalNo,
    string Address,
    bool Gender,
    CancellationToken ct)
  {
    var person = await _unitOfWork.People.GetByIdAsync(personId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person {personId} not found for update.", personId);
      return ApplicationErrors.PersonNotFound;
    }

    if (!string.IsNullOrWhiteSpace(NationalNo))
    {
      var existing = await _unitOfWork.People.GetByNationalNoAsync(NationalNo.Trim(), ct);

      if (existing is not null && existing.Id != personId)
      {
        _logger.LogWarning("National number already exists for another person: {NationalNo}", NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }

    var updateResult = person.Update(
      Fullname.Trim(),
      Birthdate,
      Phone.Trim(),
      NationalNo?.Trim(),
      Address.Trim(),
      Gender);

    if (updateResult.IsError)
    {
      _logger.LogWarning("Update failed for Person {PersonId}: {Error}", personId, updateResult.Errors);
      return updateResult.Errors;
    }

    _logger.LogInformation("Person domain entity prepared to be updated (not persisted yet).");

    return person;
  }
}