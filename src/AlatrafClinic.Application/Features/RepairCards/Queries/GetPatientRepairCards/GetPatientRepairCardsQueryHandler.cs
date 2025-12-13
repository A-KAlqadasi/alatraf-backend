using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPatientRepairCards;

public class GetPatientRepairCardsQueryHandler : IRequestHandler<GetPatientRepairCardsQuery, Result<List<RepairCardDiagnosisDto>>>
{
    private readonly ILogger<GetPatientRepairCardsQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetPatientRepairCardsQueryHandler(ILogger<GetPatientRepairCardsQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<RepairCardDiagnosisDto>>> Handle(GetPatientRepairCardsQuery query, CancellationToken ct)
    {
        var repairCards = await _context.RepairCards
            .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.DiagnosisIndustrialParts).ThenInclude(d=> d.IndustrialPartUnit)
            .Include(tc => tc.DiagnosisIndustrialParts).ThenInclude(d=> d.IndustrialPartUnit).ThenInclude(d=> d.Unit)
            .Where(tc=> tc.Diagnosis.PatientId == query.PatientId).ToListAsync(ct);
            

        if (!repairCards.Any())
        {
            return RepairCardErrors.NoRepairCardsForPaitent;
        }

        return repairCards.ToDiagnosisDtos();
    }
}