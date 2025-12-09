using Microsoft.Extensions.Logging;
using MediatR;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Application.Features;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public class GetTherapyCardByIdWithSessionsQueryHandler
    : IRequestHandler<GetTherapyCardByIdWithSessionsQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetTherapyCardByIdWithSessionsQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapyCardByIdWithSessionsQueryHandler(ILogger<GetTherapyCardByIdWithSessionsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TherapyCardDto>> Handle(GetTherapyCardByIdWithSessionsQuery query, CancellationToken ct)
    {
        var card = await _unitOfWork.TherapyCards.GetByIdAsync(query.TherapyCardId, ct);
        
        if (card is null)
        {
            _logger.LogWarning("Therapy card with ID {TherapyCardId} not found.", query.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }

        return card.ToDto();
    }
}