using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;

    public DeleteTicketCommandHandler(IAppDbContext context, ILogger<DeleteTicketCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Deleted>> Handle(DeleteTicketCommand command, CancellationToken ct)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { command.TicketId }, ct);
        if (ticket is null)
        {
            _logger.LogError("Ticket with Id {TicketId} not found.", command.TicketId);
            return TicketErrors.TicketNotFound;
        }
        if (!ticket.IsEditable)
        {
            _logger.LogError("Ticket with Id {TicketId} is not editable and cannot be deleted.", command.TicketId);
            return TicketErrors.ReadOnly;
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Ticket with Id {TicketId} deleted successfully.", command.TicketId);
        return Result.Deleted;
    }
}