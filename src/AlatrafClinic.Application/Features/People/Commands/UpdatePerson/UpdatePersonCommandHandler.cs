using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Commands.UpdatePerson;

public class UpdatePersonCommandHandler(
    ILogger<UpdatePersonCommandHandler> _logger,
    IAppDbContext _context
    )
    : IRequestHandler<UpdatePersonCommand, Result<Updated>>
{

    public async Task<Result<Updated>> Handle(UpdatePersonCommand command, CancellationToken ct)
    {

        var person = await _context.People.FirstOrDefaultAsync(p=> p.Id == command.PersonId, ct);
        if (person is null)
        {
        _logger.LogWarning("Person {PersonId} not found for update.", command.PersonId);
        return ApplicationErrors.PersonNotFound;
        }

        if (!string.IsNullOrWhiteSpace(command.NationalNo))
        {
            var existing = await _context.People.FirstOrDefaultAsync(p => p.NationalNo == command.NationalNo, ct);

            if (existing is not null && existing.Id != command.PersonId)
            {
                _logger.LogWarning("National number already exists for another person: {NationalNo}", command.NationalNo);
                return PersonErrors.NationalNoExists;
            }
        }
        
        var updateResult = person.Update(
            command.Fullname.Trim(),
            command.Birthdate,
            command.Phone.Trim(),
            command.NationalNo?.Trim(),
            command.Address.Trim(), command.Gender);

        if (updateResult.IsError)
        {
        _logger.LogWarning("Update failed for Person {PersonId}: {Error}", command.PersonId, updateResult.Errors);
        return updateResult.Errors;
        }
        _context.People.Update(person);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Person updated successfully with ID: {PersonId}", person.Id);

        return Result.Updated;
    }
}