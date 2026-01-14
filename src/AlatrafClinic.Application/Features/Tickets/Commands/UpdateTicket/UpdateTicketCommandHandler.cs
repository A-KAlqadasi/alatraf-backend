using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, Result<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(IAppDbContext context, ILogger<UpdateTicketCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(UpdateTicketCommand command, CancellationToken ct)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { command.TicketId }, ct);
        if (ticket is null)
        {
            _logger.LogError("Ticket with Id {TicketId} not found.", command.TicketId);
            
            return TicketErrors.TicketNotFound;
        }

        var patient = await _context.Patients.FindAsync(new object[] { command.PatientId }, ct);
        if (patient is null)
        {
            _logger.LogError("Patient with Id {PatientId} not found.", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var service = await _context.Services.FindAsync(new object[] { command.ServiceId }, ct);
        if (service is null)
        {
            _logger.LogError("Service with Id {ServiceId} not found.", command.ServiceId);
            return Domain.Services.ServiceErrors.ServiceNotFound;
        }

        var updateResult = ticket.Update(patient, service, command.Status);
        if (updateResult.IsError)
        {
            _logger.LogError("Failed to update ticket Id {TicketId}: {Error}", command.TicketId, updateResult.Errors);
            return updateResult.Errors;
        }
        
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Ticket with Id {TicketId} updated successfully.", command.TicketId);
        
        return Result.Updated;
    }
}