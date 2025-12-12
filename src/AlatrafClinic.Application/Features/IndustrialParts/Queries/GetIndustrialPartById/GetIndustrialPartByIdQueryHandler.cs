using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartById;

public class GetIndustrialPartByIdQueryHandler : IRequestHandler<GetIndustrialPartByIdQuery, Result<IndustrialPartDto>>
{
    private readonly ILogger<GetIndustrialPartByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetIndustrialPartByIdQueryHandler(ILogger<GetIndustrialPartByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<IndustrialPartDto>> Handle(GetIndustrialPartByIdQuery query, CancellationToken ct)
    {
        var industrialPart = await _context.IndustrialParts.Include(i=> i.IndustrialPartUnits).FirstOrDefaultAsync(i=> i.Id == query.IdustrialPartId, ct);

        if (industrialPart is null)
        {
            _logger.LogError("Industrial part with ID {IndustrialPartId} not found.", query.IdustrialPartId);

            return IndustrialPartErrors.IndustrialPartNotFound;
        }
        
        return industrialPart.ToDto();
    }
}