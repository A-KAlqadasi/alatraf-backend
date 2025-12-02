

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using FluentValidation;

using MechanicShop.Application.Common.Errors;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;

public class PersonUpdateService : IPersonUpdateService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<PersonUpdateService> _logger;
  private readonly IValidator<PersonInput> _validator; // optional, same idea

  public PersonUpdateService(
      IUnitOfWork unitOfWork,
      ILogger<PersonUpdateService> logger,
        IValidator<PersonInput> validator)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
    _validator = validator;

  }

  public async Task<Result<Person>> UpdateAsync(
      int personId,
       PersonInput updatePerson,

      CancellationToken ct)
  {
    var person = await _unitOfWork.People.GetByIdAsync(personId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person {personId} not found for update.", personId);
      return ApplicationErrors.PersonNotFound;
    }

    if (!string.IsNullOrWhiteSpace(updatePerson.NationalNo))
    {
      var existing = await _unitOfWork.People.GetByNationalNoAsync(updatePerson.NationalNo.Trim(), ct);

      if (existing is not null && existing.Id != personId)
      {
        _logger.LogWarning("National number already exists for another person: {NationalNo}", updatePerson.NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }

    var updateResult = person.Update(
      updatePerson.Fullname.Trim(),
      updatePerson.Birthdate,
      updatePerson.Phone.Trim(),
      updatePerson.NationalNo?.Trim(),
      updatePerson.Address.Trim(),

      person.Gender);

    if (updateResult.IsError)
    {
      _logger.LogWarning("Update failed for Person {PersonId}: {Error}", personId, updateResult.Errors);
      return updateResult.Errors;
    }

    _logger.LogInformation("Person domain entitynprepered to  updated  (not persisted yet).");

    return person;
  }
}