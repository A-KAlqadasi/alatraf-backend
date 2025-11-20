
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result<AppointmentDto>>
{
    private readonly ILogger<ScheduleAppointmentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly AppointmentScheduleRules _rules;
    private readonly HolidayCalendar _holidayCalendar;

    public ScheduleAppointmentCommandHandler(ILogger<ScheduleAppointmentCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache, AppointmentScheduleRules rules, HolidayCalendar holidayCalendar)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _rules = rules;
        _holidayCalendar = holidayCalendar;
    }

    public async Task<Result<AppointmentDto>> Handle(ScheduleAppointmentCommand command, CancellationToken ct)
    {
        Ticket? ticket = await _unitOfWork.Tickets.GetByIdAsync(command.TicketId, ct);
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

        DateTime lastAppointmentDate = await _unitOfWork.Appointments.GetLastAppointmentDate(ct);
        

        var appointmentResult = Appointment.Schedule(
            ticketId: ticket.Id,
            patientType: command.PatientType,
            requestedDate: command.RequestedDate,
            notes: command.Notes,
            lastScheduledDate: lastAppointmentDate,
            rules: _rules,
            holidays: _holidayCalendar
        );
        
        if (appointmentResult.IsError)
        {
            _logger.LogError("Failed to schedule appointment for Ticket {ticketId}. Error: {error}", command.TicketId, appointmentResult.TopError);
            return appointmentResult.Errors;
        }
        Appointment appointment = appointmentResult.Value;
        appointment.Ticket = ticket;
        ticket.Pause();

        await _unitOfWork.Appointments.AddAsync(appointment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Appointment {appointmentId} scheduled for Ticket {ticketId} on {attendDate}", appointment.Id, ticket.Id, appointment.AttendDate);

        return appointment.ToDto();
    }
}