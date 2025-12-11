using Microsoft.Extensions.Logging;
using MediatR;

using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public class GetTherapyCardByIdWithSessionsQueryHandler
    : IRequestHandler<GetTherapyCardByIdWithSessionsQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetTherapyCardByIdWithSessionsQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetTherapyCardByIdWithSessionsQueryHandler(ILogger<GetTherapyCardByIdWithSessionsQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<TherapyCardDto>> Handle(GetTherapyCardByIdWithSessionsQuery query, CancellationToken ct)
    {
        var card = await _context.TherapyCards
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjuryTypes)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjurySides)
            .Include(tc => tc.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .Include(tc => tc.Sessions)
                .ThenInclude(tc=> tc.SessionPrograms)
                    .ThenInclude(tc=> tc.DiagnosisProgram)
                        .ThenInclude(tc=> tc.MedicalProgram)
            .AsNoTracking()
            .FirstOrDefaultAsync(tc=> tc.Id == query.TherapyCardId, ct);
        
        if (card is null)
        {
            _logger.LogWarning("Therapy card with ID {TherapyCardId} not found.", query.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }

        return card.ToDto();
    }
}