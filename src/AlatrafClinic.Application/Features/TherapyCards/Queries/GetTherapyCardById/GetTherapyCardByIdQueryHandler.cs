using Microsoft.Extensions.Logging;
using MediatR;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public class GetTherapyCardByIdQueryHandler
    : IRequestHandler<GetTherapyCardByIdQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetTherapyCardByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapyCardByIdQueryHandler(ILogger<GetTherapyCardByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TherapyCardDto>> Handle(GetTherapyCardByIdQuery query, CancellationToken ct)
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