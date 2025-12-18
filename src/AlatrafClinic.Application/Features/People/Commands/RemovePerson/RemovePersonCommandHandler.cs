using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Commands.RemovePerson;
public class RemovePersonCommandHandler(
    ILogger<RemovePersonCommandHandler> _logger,
    IAppDbContext _context
    )
    : IRequestHandler<RemovePersonCommand, Result<Deleted>>
{

  public async Task<Result<Deleted>> Handle(RemovePersonCommand command, CancellationToken ct)
  {
    _logger.LogInformation("Attempting to delete Person with ID: {PersonId}", command.PersonId);

    var person = await _context.People.FirstOrDefaultAsync(p=> p.Id == command.PersonId, ct);
    if (person is null)
    {
      _logger.LogWarning("Person with ID {PersonId} not found for deletion.", command.PersonId);
      return ApplicationErrors.PersonNotFound;
    }
    
    _context.People.Remove(person);
    await _context.SaveChangesAsync(ct);


    _logger.LogInformation("Person {PersonId} deleted successfully.", command.PersonId);

    return Result.Deleted;
  }
}