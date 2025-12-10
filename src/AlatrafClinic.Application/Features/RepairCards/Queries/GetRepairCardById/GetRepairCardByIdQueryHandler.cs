using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;

public class GetRepairCardByIdQueryHandler : IRequestHandler<GetRepairCardByIdQuery, Result<RepairCardDiagnosisDto>>
{
    private readonly ILogger<GetRepairCardByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetRepairCardByIdQueryHandler(ILogger<GetRepairCardByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<RepairCardDiagnosisDto>> Handle(GetRepairCardByIdQuery query, CancellationToken ct)
    {
        var repairCard = await _context.RepairCards
        .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjurySides)
        .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjuryReasons)
        .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjuryTypes)
        .Include(r=> r.Diagnosis)
                .ThenInclude(t=> t.Patient)
                    .ThenInclude(p=> p.Person)
        .Include(r=> r.DiagnosisIndustrialParts)
            .ThenInclude(i=> i.IndustrialPartUnit)
                .ThenInclude(u=> u.IndustrialPart)
        .Include(r=> r.DiagnosisIndustrialParts)
            .ThenInclude(i=> i.IndustrialPartUnit)
                .ThenInclude(u=> u.Unit)
        .AsNoTracking()
        .FirstOrDefaultAsync(r=> r.Id ==query.RepairCardId, ct);

        if (repairCard is null)
        {
            _logger.LogError("Repair card with ID {RepairCardId} not found.", query.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        return repairCard.ToDiagnosisDto();
    }
}