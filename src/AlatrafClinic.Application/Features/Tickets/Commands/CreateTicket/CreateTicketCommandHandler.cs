using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<TicketDto>>
{
    private readonly ILogger<CreateTicketCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public CreateTicketCommandHandler(ILogger<CreateTicketCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<TicketDto>> Handle(CreateTicketCommand command, CancellationToken ct)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(command.ServiceId, ct);

        if (service is null)
        {
            _logger.LogError("Service with Id {ServiceId} not found.", command.ServiceId);

            return Domain.Services.ServiceErrors.ServiceNotFound;
        }
        Patient? patient = null;

        if(service.Id != 1)
        {
            if (command.PatientId is null)
            {
                _logger.LogError("PatientId is required for ServiceId {ServiceId}.", command.ServiceId);
                return TicketErrors.PatientIsRequired;
            }

            patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId.Value, ct);

            if (patient is null)
            {
                _logger.LogError("Patient with Id {PatientId} not found.", command.PatientId);

                return PatientErrors.PatientNotFound;
            }
            
        }
        
        var ticketResult = Ticket.Create(patient, service);
        

        if (ticketResult.IsError)
        {
            _logger.LogError("Failed to create ticket: {Error}", ticketResult.Errors);
            return ticketResult.Errors;
        }
        var ticket = ticketResult.Value;
        
        await _unitOfWork.Tickets.AddAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        await _cache.RemoveByTagAsync("ticket", ct);
        _logger.LogInformation("Ticket with Id {TicketId} created successfully.", ticket.Id);
        
        return ticket.ToDto();
    }
}