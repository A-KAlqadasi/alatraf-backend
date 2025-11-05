
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MechanicShop.Application.Common.Errors;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.RemovePerson;
public class RemovePersonCommandHandler(
    ILogger<RemovePersonCommandHandler> logger,
    IUnitOfWork unitWork,
    HybridCache cache
    )
    : IRequestHandler<RemovePersonCommand, Result<Deleted>>
{
  private readonly ILogger<RemovePersonCommandHandler> _logger = logger;
  private readonly IUnitOfWork _unitWork = unitWork;
  private readonly HybridCache _cache = cache;

  public async Task<Result<Deleted>> Handle(RemovePersonCommand command, CancellationToken ct)
  {
    _logger.LogInformation("Attempting to delete Person with ID: {PersonId}", command.PersonId);

    var person = await _unitWork.Person.GetByIdAsync(command.PersonId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person with ID {PersonId} not found for deletion.", command.PersonId);
      return ApplicationErrors.PersonNotFound;
    }

    var hasReferences = await _unitWork.Person.HasReferencesAsync(command.PersonId, ct);
    if (hasReferences)
    {
      _logger.LogWarning(
                "Person {PersonId} cannot be deleted because they are referenced by another entity (Doctor, Patient, or Employee).",
                command.PersonId);
      return ApplicationErrors.CannotDeleteReferencedPerson;
    }

    await _unitWork.Person.DeleteAsync(person, ct);
    await _unitWork.SaveChangesAsync(ct);

    await _cache.RemoveByTagAsync("person", ct);

    _logger.LogInformation("Person {PersonId} deleted successfully.", command.PersonId);

    return Result.Deleted;
  }
}
