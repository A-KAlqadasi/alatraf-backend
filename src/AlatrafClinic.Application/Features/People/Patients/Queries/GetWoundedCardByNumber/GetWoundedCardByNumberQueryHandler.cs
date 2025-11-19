using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCardByNumber;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetWoundedCardByNumber;

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
        var card = await _unitOfWork.Patients.GetWoundedCardByNumber(query.CardNumber, ct);
        if (card is null)
        {
            _logger.LogError("Wounded card with number {number} is not found", query.CardNumber);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        return card.ToDto();
    }
}