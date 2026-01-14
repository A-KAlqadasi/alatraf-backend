
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Application.Features.Appointments.Services;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Appointments.SchedulingRulesService;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public sealed class ScheduleAppointmentCommandHandler
    : IRequestHandler<ScheduleAppointmentCommand, Result<AppointmentDto>>
{
    private readonly ILogger<ScheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly AppointmentSchedulingService _schedulingService;
    
    public ScheduleAppointmentCommandHandler(
        ILogger<ScheduleAppointmentCommandHandler> logger,
        IAppDbContext context,
        AppointmentSchedulingService schedulingService)
    {
        _logger = logger;
        _context = context;
        _schedulingService = schedulingService;
    }
    
    public async Task<Result<AppointmentDto>> Handle(
        ScheduleAppointmentCommand command, 
        CancellationToken ct)
    {
        // 1. Load ticket
        var ticket = await LoadTicketOrFail(command.TicketId, ct);
        if (ticket.IsError) return ticket.Errors;
        
        // 2. Business validation
        if (!ticket.Value.IsEditable)
        {
            _logger.LogError("Ticket {TicketId} is not editable!", command.TicketId);
            return TicketErrors.ReadOnly;
        }
        
        if (ticket.Value.Status == TicketStatus.Pause)
        {
            _logger.LogWarning("Ticket {TicketId} is already scheduled", command.TicketId);
            return TicketErrors.TicketAlreadHasAppointment;
        }
        
        // 3. Get scheduling rules and context
        var rules = await _schedulingService.GetSchedulingRulesAsync(ct);
        var lastAppointmentDate = await _schedulingService.GetLastAppointmentDateAsync(ct);
        
        var context = SchedulingContext.Create(
            AlatrafClinicConstants.TodayDate,
            lastAppointmentDate,
            rules);
        
        // 4. Find next available date
        DateOnly appointmentDate;
        try
        {
            appointmentDate = await _schedulingService.GetNextValidDateAsync(
                context,
                async date => await _schedulingService.GetAppointmentCountForDateAsync(date, null, ct));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "No available appointment dates found for Ticket {TicketId}", command.TicketId);
            return Error.Failure("Appointment.NoAvailableDates", ex.Message);
        }
        
        // 5. Create appointment
        var appointmentResult = Appointment.Schedule(
            ticketId: ticket.Value.Id,
            patientType: ticket.Value.Patient!.PatientType,
            attendDate: appointmentDate,
            notes: command.Notes);
        
        if (appointmentResult.IsError)
        {
            _logger.LogError(
                "Failed to schedule appointment for Ticket {TicketId}. Error: {Error}",
                command.TicketId,
                appointmentResult.TopError);
            return appointmentResult.Errors;
        }
        
        var appointment = appointmentResult.Value;
        
        // 6. Update relationships
        appointment.Ticket = ticket.Value;
        ticket.Value.Pause();
        
        await _context.Appointments.AddAsync(appointment, ct);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation(
            "Appointment {AppointmentId} scheduled for Ticket {TicketId} on {AttendDate}",
            appointment.Id,
            ticket.Value.Id,
            appointment.AttendDate);
        
        return appointment.ToDto();
    }
    
    private async Task<Result<Ticket>> LoadTicketOrFail(int ticketId, CancellationToken ct)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Patient!)
                .ThenInclude(p => p.Person)
            .FirstOrDefaultAsync(t => t.Id == ticketId, ct);
        
        if (ticket is null)
        {
            _logger.LogError("Ticket {TicketId} is not found!", ticketId);
            return TicketErrors.TicketNotFound;
        }
        
        return ticket;
    }
}
