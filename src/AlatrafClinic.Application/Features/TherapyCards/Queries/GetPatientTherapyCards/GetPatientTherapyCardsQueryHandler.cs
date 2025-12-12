using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetPatientTherapyCards;

public class GetPatientTherapyCardsQueryHandler : IRequestHandler<GetPatientTherapyCardsQuery, Result<List<TherapyCardDiagnosisDto>>>
{
    private readonly ILogger<GetPatientTherapyCardsQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetPatientTherapyCardsQueryHandler(ILogger<GetPatientTherapyCardsQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<TherapyCardDiagnosisDto>>> Handle(GetPatientTherapyCardsQuery query, CancellationToken ct)
    {
        var therapyCards = await _context.TherapyCards
            .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.DiagnosisPrograms).ThenInclude(d=> d.MedicalProgram)
            .Where(tc=> tc.Diagnosis.PatientId == query.PatientId).ToListAsync(ct);
            

        if (!therapyCards.Any())
        {
            return TherapyCardErrors.NoTherapyCardsFoundForPatient;
        }

        return therapyCards.ToTherapyDiagnosisDtos();
    }
}