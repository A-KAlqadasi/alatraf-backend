using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetLastActiveTherapyCard;

public class GetLastActiveTherapyCardQueryHandler
    : IRequestHandler<GetLastActiveTherapyCardQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetLastActiveTherapyCardQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetLastActiveTherapyCardQueryHandler(
        ILogger<GetLastActiveTherapyCardQueryHandler> logger,
        IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<TherapyCardDto>> Handle(
        GetLastActiveTherapyCardQuery query,
        CancellationToken ct)
    {

        var isExists = await _context.Patients
            .AnyAsync(p=> p.Id == query.PatientId, ct);
        
        if (!isExists)
        {
            _logger.LogWarning(
                "Patient with ID {PatientId} not found",
                query.PatientId);

            return PatientErrors.PatientNotFound;
        }

        var therapyCard = await _context.TherapyCards
            .Include(t=> t.Diagnosis)
            .OrderByDescending(tc => tc.CreatedAtUtc.DateTime)
            .FirstOrDefaultAsync(tc => tc.Diagnosis.PatientId == query.PatientId && tc.IsActive, ct);
            
        if (therapyCard is null)
        {
            _logger.LogWarning(
                "No active therapy card found for patient with ID {PatientId}",
                query.PatientId);

            return TherapyCardErrors.NoActiveTherapyCardFound;
        }

        return therapyCard.ToDto();
    }
}