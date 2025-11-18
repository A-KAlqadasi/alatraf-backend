
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCardByNumber;

public class GetDisabledCardByNumberQueryHandler : IRequestHandler<GetDisabledCardByNumberQuery, Result<DisabledCardDto>>
{
    private readonly ILogger<GetDisabledCardByNumberQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public GetDisabledCardByNumberQueryHandler(ILogger<GetDisabledCardByNumberQueryHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<DisabledCardDto>> Handle(GetDisabledCardByNumberQuery query, CancellationToken ct)
    {
        var card = await _unitOfWork.Patients.GetDisabledCardByNumber(query.CardNumber, ct);
        if (card is null)
        {
            _logger.LogError("Disabled card with number {number} is not found", query.CardNumber);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        return card.ToDto();
    }
}