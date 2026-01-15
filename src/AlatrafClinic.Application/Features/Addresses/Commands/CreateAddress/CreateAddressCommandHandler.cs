using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler(
    IAppDbContext _context
    ) : IRequestHandler<CreateAddressCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(CreateAddressCommand command, CancellationToken ct)
    {
        var addressExists = _context.Addresses.AnyAsync(a => a.Name == command.Name.Trim(), ct);
        if (await addressExists)
        {
            return Result.Created;
        }
        var addressExistsById = await _context.Addresses.AnyAsync(a => a.Id == command.Id, ct);
        if (addressExistsById)
        {
            
            return PersonErrors.AddressIdIsAlreadyInUse;
        }

        var address = Address.Create(command.Id, command.Name.Trim());

        await _context.Addresses.AddAsync(address.Value, ct);
        await  _context.SaveChangesAsync(ct);

        return Result.Created;
    }
}
