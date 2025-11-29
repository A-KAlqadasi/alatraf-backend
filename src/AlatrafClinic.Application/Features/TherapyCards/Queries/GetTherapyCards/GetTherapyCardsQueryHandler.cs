using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public class GetTherapyCardsQueryHandler
    : IRequestHandler<GetTherapyCardsQuery, Result<PaginatedList<TherapyCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapyCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<TherapyCardDto>>> Handle(GetTherapyCardsQuery query, CancellationToken ct)
    {
        
        var spec = new TherapyCardsFilter(query);

        var totalCount = await _unitOfWork.TherapyCards.CountAsync(spec, ct);

        var cards = await _unitOfWork.TherapyCards
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = cards
            .Select(tc => tc.ToDto())
            .ToList();

        return new PaginatedList<TherapyCardDto>
        {
            Items      = items,
            PageNumber = spec.Page,
            PageSize   = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };
    }
}