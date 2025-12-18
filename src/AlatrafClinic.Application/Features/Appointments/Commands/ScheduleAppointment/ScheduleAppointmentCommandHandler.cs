
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result<AppointmentDto>>
{
    private readonly ILogger<ScheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;


    public ScheduleAppointmentCommandHandler(ILogger<ScheduleAppointmentCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<AppointmentDto>> Handle(ScheduleAppointmentCommand command, CancellationToken ct)
    {
        Ticket? ticket = await _context.Tickets
        .Include(t=> t.Patient!)
            .ThenInclude(t=> t.Person)
        .FirstOrDefaultAsync(t=> t.Id == command.TicketId, ct);

        if (ticket is null)
        {
            _logger.LogError("Ticket {ticketId} is not found!", command.TicketId);
            return TicketErrors.TicketNotFound;
        }

        if (!ticket.IsEditable)
        {
            _logger.LogError("Ticket {ticketId} is not editable!", command.TicketId);
            return TicketErrors.ReadOnly;
        }

        if (ticket.Status == TicketStatus.Pause)
        {
            _logger.LogWarning("Ticket {ticketId} is already scheduled", command.TicketId);
            return TicketErrors.TicketAlreadHasAppointment;
        }

        var lastAppointment = await _context.Appointments.OrderByDescending(a=> a.AttendDate).FirstOrDefaultAsync(ct);

        DateOnly lastAppointmentDate = lastAppointment?.AttendDate ?? DateOnly.MinValue;

        DateOnly baseDate = lastAppointmentDate < AlatrafClinicConstants.TodayDate ? AlatrafClinicConstants.TodayDate : lastAppointmentDate;

        if (command.RequestedDate.HasValue && command.RequestedDate.Value > baseDate)
        {
            baseDate = command.RequestedDate.Value;
        }

        var allowedDaysString = await _context.AppSettings
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);
        
        
        var allowedDays = allowedDaysString?.Split(',').Select(day => Enum.Parse<DayOfWeek>(day.Trim())).ToList() ?? [DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday];

        var holidays = await _context.Holidays.ToListAsync(ct);


        while (!allowedDays.Contains(baseDate.DayOfWeek) || baseDate.DayOfWeek == DayOfWeek.Friday || holidays.Any(h => h.Matches(baseDate)))
        {
            baseDate = baseDate.AddDays(1);
        }

        var appointmentResult = Appointment.Schedule(
            ticketId: ticket.Id,
            patientType: ticket.Patient!.PatientType,
            attendDate: baseDate,
            notes: command.Notes
        );
        
        if (appointmentResult.IsError)
        {
            _logger.LogError("Failed to schedule appointment for Ticket {ticketId}. Error: {error}", command.TicketId, appointmentResult.TopError);
            return appointmentResult.Errors;
        }
        Appointment appointment = appointmentResult.Value;
        appointment.Ticket = ticket;
        ticket.Pause();

        await _context.Appointments.AddAsync(appointment, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Appointment {appointmentId} scheduled for Ticket {ticketId} on {attendDate}", appointment.Id, ticket.Id, appointment.AttendDate);

        return appointment.ToDto();
    }
    
}