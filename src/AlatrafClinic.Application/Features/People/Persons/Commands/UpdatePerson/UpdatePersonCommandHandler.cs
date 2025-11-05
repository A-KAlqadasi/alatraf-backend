
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.UpdatePerson;

public class UpdatePersonCommandHandler(
    ILogger<UpdatePersonCommandHandler> logger,
    IUnitOfWork unitWork
    )
    : IRequestHandler<UpdatePersonCommand, Result<Updated>>
{
  private readonly ILogger<UpdatePersonCommandHandler> _logger = logger;
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<Updated>> Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
  {

    var person = await _unitWork.Person.GetByIdAsync(command.PersonId, cancellationToken);
    if (person is null)
    {
      _logger.LogWarning("Person {PersonId} not found for update.", command.PersonId);
      return ApplicationErrors.PersonNotFound;
    }

    if (!string.IsNullOrWhiteSpace(command.NationalNo))
    {
      var existing = await _unitWork.Person.GetByNationalNoAsync(command.NationalNo, cancellationToken);

      if (existing is not null && existing.Id != command.PersonId)
      {
        _logger.LogWarning("National number already exists for another person: {NationalNo}", command.NationalNo);
        return PersonErrors.NationalNoExists;
      }
    }
    var updateResult = person.Update(
        command.Fullname.Trim(),
        command.Birthdate,
        command.Phone?.Trim(),
        command.NationalNo?.Trim(),
        command.Address?.Trim());

    if (updateResult.IsError)
    {
      _logger.LogWarning("Update failed for Person {PersonId}: {Error}", command.PersonId, updateResult.Errors);
      return updateResult.Errors;
    }
    await _unitWork.Person.UpdateAsync(person, cancellationToken);
    await _unitWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Person updated successfully with ID: {PersonId}", person.Id);

    return Result.Updated;
  }
}