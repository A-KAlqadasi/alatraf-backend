using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetWoundedCards;

public class GetWoundedCardsQueryHandler : IRequestHandler<GetWoundedCardsQuery, Result<List<WoundedCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWoundedCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<WoundedCardDto>>> Handle(GetWoundedCardsQuery query, CancellationToken ct)
    {
        var cards = await _unitOfWork.Patients.GetWoundedCardsAsync(ct);

        return cards.ToDtos();
    }
}