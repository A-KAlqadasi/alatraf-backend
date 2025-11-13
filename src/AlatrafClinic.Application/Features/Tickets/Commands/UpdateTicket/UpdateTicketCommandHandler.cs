using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, Result<Updated>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(UpdateTicketCommand command, CancellationToken ct)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(command.TicketId, ct);
        if (ticket is null)
        {
            _logger.LogError("Ticket with Id {TicketId} not found.", command.TicketId);
            
            return TicketErrors.TicketNotFound;
        }

        var patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
        {
            _logger.LogError("Patient with Id {PatientId} not found.", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var service = await _unitOfWork.Services.GetByIdAsync(command.ServiceId, ct);
        if (service is null)
        {
            _logger.LogError("Service with Id {ServiceId} not found.", command.ServiceId);
            return Domain.Services.ServiceErrors.ServiceNotFound;
        }

        var updateResult = ticket.Update(patient, service);
        if (updateResult.IsError)
        {
            _logger.LogError("Failed to update ticket Id {TicketId}: {Error}", command.TicketId, updateResult.Errors);
            return updateResult.Errors;
        }

        await _unitOfWork.Tickets.UpdateAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Ticket with Id {TicketId} updated successfully.", command.TicketId);
        
        return Result.Updated;
    }
}