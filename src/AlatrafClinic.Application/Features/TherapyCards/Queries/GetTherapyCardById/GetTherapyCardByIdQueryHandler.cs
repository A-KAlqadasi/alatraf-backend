using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public class GetTherapyCardByIdQueryHandler : IRequestHandler<GetTherapyCardByIdQuery, Result<TherapyCardDiagnosisDto>>
{
    private readonly ILogger<GetTherapyCardByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetTherapyCardByIdQueryHandler(ILogger<GetTherapyCardByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<TherapyCardDiagnosisDto>> Handle(GetTherapyCardByIdQuery query, CancellationToken ct)
    {
        var therpayCard = await _context.TherapyCards
            .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.Diagnosis).ThenInclude(d=> d.DiagnosisPrograms)
            .Include(tc => tc.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
            .FirstOrDefaultAsync(tc => tc.Id == query.TherapyCardId, ct);

        if(therpayCard is null)
        {
            _logger.LogError("Therpay card with Id {id} is not found", query.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }

        return therpayCard.ToTherapyDiagnosisDto();
    }
}