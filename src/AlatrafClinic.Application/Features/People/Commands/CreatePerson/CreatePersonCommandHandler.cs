using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Commands.CreatePerson;

public  class CreatePersonCommandHandler(
    ILogger<CreatePersonCommandHandler> _logger,
    IAppDbContext _context
    ) : IRequestHandler<CreatePersonCommand, Result<PersonDto>>
{

    public async Task<Result<PersonDto>> Handle(CreatePersonCommand command, CancellationToken ct)
    {

        if (!string.IsNullOrWhiteSpace(command.NationalNo))
        {
            var existing = await _context.People
                .AnyAsync(p => p.NationalNo == command.NationalNo, ct);

            if (existing)
            {
                _logger.LogWarning("Person creation aborted. National number already exists: {NationalNo}", command.NationalNo);
                return PersonErrors.NationalNoExists;
            }
        }
        var isAddressExists =  await _context.Addresses
            .AnyAsync(a => a.Id == command.AddressId, ct);
        if (!isAddressExists)
        {
            _logger.LogWarning("Person creation aborted. Address not found: {AddressId}", command.AddressId);
            return PersonErrors.AddressNotFound;
        }
        
        var createResult = Person.Create(
            command.Fullname.Trim(),
            command.Birthdate,
            command.Phone.Trim(),
            command.NationalNo?.Trim(),
            command.AddressId,
            command.Gender);

        if (createResult.IsError)
        {
        return createResult.Errors;
        }
        var person = createResult.Value;

        await _context.People.AddAsync(person, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Person created successfully with ID: {PersonId}", person.Id);

        return person.ToDto();
    
    }
}