using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketDto>>
{
    private readonly ILogger<GetTicketByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketByIdQueryHandler(ILogger<GetTicketByIdQueryHandler> logger ,IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<TicketDto>> Handle(GetTicketByIdQuery query, CancellationToken ct)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(query.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with ID {TicketId} not found.", query.ticketId);
            return TicketErrors.TicketNotFound;
        }

        return ticket.ToDto();
    }
}