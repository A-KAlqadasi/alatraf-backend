using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsForDropdown;

public sealed class GetIndustrialPartsForDropdownQueryHandler
    : IRequestHandler<GetIndustrialPartsForDropdownQuery, Result<List<IndustrialPartDto>>>
{
    private readonly IAppDbContext _context;

    public GetIndustrialPartsForDropdownQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<IndustrialPartDto>>> Handle(
        GetIndustrialPartsForDropdownQuery query,
        CancellationToken ct)
    {
        var data = await _context.IndustrialParts.Include(i=> i.IndustrialPartUnits).OrderBy(x=> x.Name).ToListAsync();

        if(data is null || !data.Any())
        {
            return IndustrialPartErrors.NoIndustrialPartsFound;
        }

        return data.ToDtos();
    }
}