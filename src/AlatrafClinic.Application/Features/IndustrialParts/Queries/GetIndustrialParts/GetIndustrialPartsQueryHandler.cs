using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed class GetIndustrialPartsQueryHandler
    : IRequestHandler<GetIndustrialPartsQuery, Result<List<IndustrialPartDto>>>
{
    private readonly IAppDbContext _context;

    public GetIndustrialPartsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<IndustrialPartDto>>> Handle(
        GetIndustrialPartsQuery query,
        CancellationToken ct)
    {
        var parts = await _context.IndustrialParts
        .Include(i=> i.IndustrialPartUnits)
            .ThenInclude(i=> i.Unit)
        .ToListAsync(ct);

        return parts.ToDtos();
    }
}