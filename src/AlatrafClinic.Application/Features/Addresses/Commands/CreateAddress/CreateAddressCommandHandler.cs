using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler(
    IAppDbContext _context
    ) : IRequestHandler<CreateAddressCommand, Result<AddressDto>>
{
    public async Task<Result<AddressDto>> Handle(CreateAddressCommand command, CancellationToken ct)
    {
        var addressExists = _context.Addresses.AnyAsync(a => a.Name == command.Name.Trim(), ct);
        if (await addressExists)
        {
            return PersonErrors.AddressNameIsAlreadyExist;
        }
        
        var maxId = await _context.Addresses.MaxAsync(a => (int?)a.Id, ct) ?? 0;
        maxId +=1;

        var address = Address.Create(maxId, command.Name.Trim());

        await _context.Addresses.AddAsync(address.Value, ct);
        await  _context.SaveChangesAsync(ct);

        return new AddressDto{
            Id = address.Value.Id,
            Name = address.Value.Name
        };
    }
}
