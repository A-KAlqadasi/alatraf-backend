using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;

    public DeleteTicketCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Deleted>> Handle(DeleteTicketCommand command, CancellationToken ct)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(command.TicketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with Id {TicketId} not found.", command.TicketId);
            return TicketErrors.TicketNotFound;
        }
        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with Id {TicketId} is not editable and cannot be deleted.", command.TicketId);
            return TicketErrors.ReadOnly;
        }
        if(await _unitOfWork.Tickets.HasAssociationsAsync(command.TicketId, ct))
        {
              _logger.LogWarning("Ticket with id {Id} has associated records and cannot be deleted", command.TicketId);
            return Error.Conflict(code: "Ticket.HasAssociations", description: $"Service with Id {command.TicketId} has associations and cannot be deleted.");
        }

        await _unitOfWork.Tickets.DeleteAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Ticket with Id {TicketId} deleted successfully.", command.TicketId);
        return Result.Deleted;
    }
}