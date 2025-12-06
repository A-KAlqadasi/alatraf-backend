using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public sealed class GetTicketsQueryHandler
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<TicketDto>>> Handle(
        GetTicketsQuery query,
        CancellationToken ct)
    {
        var specification = new TicketsFilter(query);
        

        var totalCount = await _unitOfWork.Tickets.CountAsync(specification, ct);

        var tickets = await _unitOfWork.Tickets
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = tickets.ToDtos().ToList();

        return new PaginatedList<TicketDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}