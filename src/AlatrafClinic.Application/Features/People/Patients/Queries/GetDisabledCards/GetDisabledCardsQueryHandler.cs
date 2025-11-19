
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCards;

public class GetDisabledCardsQueryHandler : IRequestHandler<GetDisabledCardsQuery, Result<List<DisabledCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDisabledCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<DisabledCardDto>>> Handle(GetDisabledCardsQuery query, CancellationToken ct)
    {
        var cards = await _unitOfWork.Patients.GetDisabledCardsAsync(ct);

        return cards.ToDtos();
    }
}