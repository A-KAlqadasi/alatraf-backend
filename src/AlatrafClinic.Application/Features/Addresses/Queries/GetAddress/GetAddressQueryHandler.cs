using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Application.Features.Addresses.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Addresses.Queries.GetAddress;

public class GetAddressQueryHandler : IRequestHandler<GetAddressQuery, Result<AddressDto>>
{
    private readonly IAppDbContext _context;

    public GetAddressQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AddressDto>> Handle(GetAddressQuery request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address is null)
        {
            return Error.NotFound("Address not found.");
        }
        
        return address.ToDto();
    }
}