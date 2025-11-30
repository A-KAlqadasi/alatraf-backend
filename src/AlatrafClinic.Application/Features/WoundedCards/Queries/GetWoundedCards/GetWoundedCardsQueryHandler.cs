using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

public class GetWoundedCardsQueryHandler
    : IRequestHandler<GetWoundedCardsQuery, Result<PaginatedList<WoundedCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWoundedCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<WoundedCardDto>>> Handle(
        GetWoundedCardsQuery query,
        CancellationToken ct)
    {
        var specification = new WoundedCardsFilter(query);

        var totalCount = await _unitOfWork.WoundedCards.CountAsync(specification, ct);

        var cards = await _unitOfWork.WoundedCards
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var today = DateTime.Today;

        var items = cards.ToDtos()
            .ToList();

        return new PaginatedList<WoundedCardDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}
