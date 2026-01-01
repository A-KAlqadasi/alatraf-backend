using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<TicketDto>>
{
    private readonly ILogger<CreateTicketCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public CreateTicketCommandHandler(ILogger<CreateTicketCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<TicketDto>> Handle(CreateTicketCommand command, CancellationToken ct)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == command.ServiceId, ct);

        if (service is null)
        {
            _logger.LogError("Service with Id {ServiceId} not found.", command.ServiceId);

            return ServiceErrors.ServiceNotFound;
        }
        Patient? patient = null;

        if(service.Id != 1)
        {
            if (command.PatientId is null)
            {
                _logger.LogError("PatientId is required for ServiceId {ServiceId}.", command.ServiceId);
                return TicketErrors.PatientIsRequired;
            }

            patient = await _context.Patients.Include(p=> p.Person).FirstOrDefaultAsync(p => p.Id == command.PatientId.Value, ct);

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
        
        await _context.Tickets.AddAsync(ticket, ct);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Ticket with Id {TicketId} created successfully.", ticket.Id);
        
        return ticket.ToDto();
    }
}