using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketDto>>
{
    private readonly ILogger<GetTicketByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetTicketByIdQueryHandler(ILogger<GetTicketByIdQueryHandler> logger ,IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<TicketDto>> Handle(GetTicketByIdQuery query, CancellationToken ct)
    {
        var ticket = await _context.Tickets
        .Include(t => t.Patient!)
            .ThenInclude(p => p.Person)
                .ThenInclude(p=> p.Address)
        .Include(t => t.Service)
        .FirstOrDefaultAsync(t => t.Id == query.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with ID {TicketId} not found.", query.ticketId);
            return TicketErrors.TicketNotFound;
        }

        return ticket.ToDto();
    }
}