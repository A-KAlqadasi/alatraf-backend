using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHandler(
    IAppDbContext _context,
    ILogger<UpdateAddressCommandHandler> _logger
    ) : IRequestHandler<UpdateAddressCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateAddressCommand command, CancellationToken ct)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == command.AddressId, ct);

        if ( address is null)
        {
            _logger.LogWarning("Address {AddressId} not found for update.", command.AddressId);
            return Error.NotFound("Address not found.");
        }
        
        address.Update(command.Name.Trim());
        _context.Addresses.Update(address);
        await  _context.SaveChangesAsync(ct);

        return Result.Updated;
    }
}