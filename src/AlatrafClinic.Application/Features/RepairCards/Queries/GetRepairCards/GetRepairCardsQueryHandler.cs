using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

public sealed class GetRepairCardsQueryHandler
    : IRequestHandler<GetRepairCardsQuery, Result<PaginatedList<RepairCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRepairCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<RepairCardDto>>> Handle(GetRepairCardsQuery query, CancellationToken ct)
    {
        var spec = new RepairCardsFilter(query);

        // total count
        var totalCount = await _unitOfWork.RepairCards.CountAsync(spec, ct);

        // entities for current page
        var cards = await _unitOfWork.RepairCards
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = cards.Select(rc => rc.ToDto()).ToList();

        var paged = new PaginatedList<RepairCardDto>
        {
            Items = items,
            PageNumber = spec.Page,
            PageSize = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };

        return paged;
    }
}
