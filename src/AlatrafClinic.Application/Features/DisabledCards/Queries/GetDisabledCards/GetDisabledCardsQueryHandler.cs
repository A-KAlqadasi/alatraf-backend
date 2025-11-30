using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

public class GetDisabledCardsQueryHandler
    : IRequestHandler<GetDisabledCardsQuery, Result<PaginatedList<DisabledCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDisabledCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<DisabledCardDto>>> Handle(
        GetDisabledCardsQuery query,
        CancellationToken ct)
    {
        var specification = new DisabledCardsFilter(query);

        var totalCount = await _unitOfWork.DisabledCards.CountAsync(specification, ct);

        var cards = await _unitOfWork.DisabledCards
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var today = DateTime.Today;

        var items = cards
            .Select(dc =>dc.ToDto())
            .ToList();

        return new PaginatedList<DisabledCardDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}