using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;
    private readonly HybridCache _cache;

    public DeleteTicketCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteTicketCommandHandler> logger, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Deleted>> Handle(DeleteTicketCommand command, CancellationToken ct)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(command.TicketId, ct);
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

        await _unitOfWork.Tickets.DeleteAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("ticket", ct);
        
        _logger.LogInformation("Ticket with Id {TicketId} deleted successfully.", command.TicketId);
        return Result.Deleted;
    }
}