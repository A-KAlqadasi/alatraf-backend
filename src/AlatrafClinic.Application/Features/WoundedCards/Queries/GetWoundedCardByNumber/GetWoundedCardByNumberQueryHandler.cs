using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCardByNumber;

public class GetWoundedCardByNumberQueryHandler : IRequestHandler<GetWoundedCardByNumberQuery, Result<WoundedCardDto>>
{
    private readonly ILogger<GetWoundedCardByNumberQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public GetWoundedCardByNumberQueryHandler(ILogger<GetWoundedCardByNumberQueryHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<WoundedCardDto>> Handle(GetWoundedCardByNumberQuery query, CancellationToken ct)
    {
        var card = await _unitOfWork.WoundedCards.GetWoundedCardByNumberAsync(query.CardNumber, ct);
        if (card is null)
        {
            _logger.LogError("Wounded card with number {number} is not found", query.CardNumber);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        return card.ToDto();
    }
}