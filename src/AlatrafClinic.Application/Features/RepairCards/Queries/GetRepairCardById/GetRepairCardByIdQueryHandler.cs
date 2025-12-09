using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;

public class GetRepairCardByIdQueryHandler : IRequestHandler<GetRepairCardByIdQuery, Result<RepairCardDto>>
{
    private readonly ILogger<GetRepairCardByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetRepairCardByIdQueryHandler(ILogger<GetRepairCardByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<RepairCardDto>> Handle(GetRepairCardByIdQuery query, CancellationToken ct)
    {
        var repairCard = await _context.RepairCards.Include(r=> r.Diagnosis).Include(r=> r.DiagnosisIndustrialParts).AsNoTracking().FirstOrDefaultAsync(r=> r.Id ==query.RepairCardId, ct);

        if (repairCard is null)
        {
            _logger.LogError("Repair card with ID {RepairCardId} not found.", query.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        return repairCard.ToDto();
    }
}