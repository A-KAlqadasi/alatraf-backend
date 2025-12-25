
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Application.Features.Appointments.Shared;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public sealed class ScheduleAppointmentCommandHandler
    : IRequestHandler<ScheduleAppointmentCommand, Result<AppointmentDto>>
{
    private readonly ILogger<ScheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public ScheduleAppointmentCommandHandler(
        ILogger<ScheduleAppointmentCommandHandler> logger,
        IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<AppointmentDto>> Handle(ScheduleAppointmentCommand command, CancellationToken ct)
    {
        var ticket = await LoadTicketOrFail(command.TicketId, ct);
        if (ticket.IsError) return ticket.Errors;

        // Business rules (as you had)
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

        // Determine the base start date
        var lastDateResult = await GetLastSchedulingPressureDate(ct);
        var lastDate = lastDateResult ?? DateOnly.MinValue;

        var today = AlatrafClinicConstants.TodayDate;

        // baseStart = max(today, lastDate, requestedDate(if any))
        var baseStart = MaxDate(today, lastDate);

        if (command.RequestedDate.HasValue)
            baseStart = MaxDate(baseStart, command.RequestedDate.Value);

        // Apply allowed days + holidays constraints
        var (allowedDays, holidays) = await LoadSchedulingRules(ct);
        var finalDate = AppointmentSchedulingCalculator.GetNextValidDateInclusive(baseStart, allowedDays, holidays);

        // Domain validation (still enforces ">= today" etc.)
        var appointmentResult = Appointment.Schedule(
            ticketId: ticket.Value.Id,
            patientType: ticket.Value.Patient!.PatientType,
            attendDate: finalDate,
            notes: command.Notes
        );

        if (appointmentResult.IsError)
        {
            _logger.LogError(
                "Failed to schedule appointment for Ticket {TicketId}. Error: {Error}",
                command.TicketId,
                appointmentResult.TopError);

            return appointmentResult.Errors;
        }

        var appointment = appointmentResult.Value;

        // Maintain relationship and state changes
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

    /// <summary>
    /// Defines which appointments contribute to “scheduling pressure” (i.e., what should affect next day).
    /// Keep it consistent across schedule + reschedule.
    /// </summary>
    private async Task<DateOnly?> GetLastSchedulingPressureDate(CancellationToken ct)
    {
        // Option A (recommended): consider only active “scheduled pipeline”
        // return await _context.Appointments.AsNoTracking()
        //     .Where(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Today)
        //     .MaxAsync(a => (DateOnly?)a.AttendDate, ct);

        // Option B (close to your current intent): exclude cancelled only
        return await _context.Appointments.AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .MaxAsync(a => (DateOnly?)a.AttendDate, ct);
    }

    private async Task<(IReadOnlyCollection<DayOfWeek> AllowedDays, IReadOnlyCollection<Holiday> Holidays)> LoadSchedulingRules(CancellationToken ct)
    {
        var allowedDaysString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);

        var allowedDays = AppointmentSchedulingCalculator.ParseAllowedDaysOrDefault(allowedDaysString);

        var holidays = await _context.Holidays.AsNoTracking().ToListAsync(ct);

        return (allowedDays, holidays);
    }

     private async Task<(IReadOnlyCollection<DayOfWeek> AllowedDays, IReadOnlyCollection<Holiday> Holidays, int DailyCapacity)>
        LoadSchedulingRulesWithCapacity(CancellationToken ct)
    {
        var allowedDaysString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);

        var allowedDays = AppointmentSchedulingCalculator.ParseAllowedDaysOrDefault(allowedDaysString);

        var holidays = await _context.Holidays.AsNoTracking().ToListAsync(ct);

        var capacityString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AppointmentDailyCapacityKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);

        var dailyCapacity = AlatrafClinicConstants.DefaultAppointmentDailyCapacity;
        if (!string.IsNullOrWhiteSpace(capacityString) && int.TryParse(capacityString, out var parsed) && parsed > 0)
            dailyCapacity = parsed;

        return (allowedDays, holidays, dailyCapacity);
    }

    private static DateOnly MaxDate(DateOnly a, DateOnly b) => a > b ? a : b;
}