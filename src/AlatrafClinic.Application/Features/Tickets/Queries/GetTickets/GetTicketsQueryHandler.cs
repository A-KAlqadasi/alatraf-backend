using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, Result<List<TicketDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetTicketsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Result<List<TicketDto>>> Handle(GetTicketsQuery query, CancellationToken ct)
    {
        var tickets = await _uow.Tickets.GetAllAsync(ct);

        return tickets.ToDtos();
    }
}