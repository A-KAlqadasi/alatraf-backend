using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Shared;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public sealed class RescheduleAppointmentCommandHandler
    : IRequestHandler<RescheduleAppointmentCommand, Result<Updated>>
{
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public RescheduleAppointmentCommandHandler(
        ILogger<RescheduleAppointmentCommandHandler> logger,
        IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Updated>> Handle(RescheduleAppointmentCommand command, CancellationToken ct)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == command.AppointmentId, ct);

        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }

        // Determine base start date
        var lastDate = await GetLastSchedulingPressureDate(ct) ?? DateOnly.MinValue;
        var today = AlatrafClinicConstants.TodayDate;

        var baseStart = MaxDate(today, lastDate);
        baseStart = MaxDate(baseStart, command.NewAttendDate);

        // --- CHANGE START: Load Capacity Rules and Find Date ---

        // 1. Load rules including capacity
        var (allowedDays, holidays, dailyCapacity) = await LoadSchedulingRulesWithCapacity(ct);

        // 2. Use the async calculator to find the next valid date that has space
        var finalDate = await AppointmentSchedulingCalculator.FindNextValidDateWithCapacityAsync(
            startInclusive: baseStart,
            allowedDays: allowedDays,
            holidays: holidays,
            dailyCapacity: dailyCapacity,
            getCountForDateAsync: async (date, token) =>
            {
                return await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(a => a.AttendDate == date
                                     && a.Status != AppointmentStatus.Cancelled
                                     && a.Status != AppointmentStatus.Absent
                                     // IMPORTANT: Exclude the current appointment from the count
                                     // so it doesn't count against the capacity if checking its own date.
                                     && a.Id != command.AppointmentId, 
                                token);
            },
            ct: ct);

        // --- CHANGE END ---

        // Domain rule: appointment must be editable, date must be >= today, etc.
        var rescheduleResult = appointment.Reschedule(finalDate);

        if (rescheduleResult.IsError)
        {
            _logger.LogWarning(
                "Failed to reschedule appointment with ID {AppointmentId}: {Error}",
                command.AppointmentId,
                rescheduleResult.TopError);
            return rescheduleResult.Errors;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Appointment {AppointmentId} rescheduled to {NewDate}",
            appointment.Id,
            appointment.AttendDate);

        return Result.Updated;
    }

    private async Task<DateOnly?> GetLastSchedulingPressureDate(CancellationToken ct)
    {
        return await _context.Appointments.AsNoTracking()
            // Updated to match the Schedule handler logic (Exclude Cancelled AND Absent)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Absent)
            .MaxAsync(a => (DateOnly?)a.AttendDate, ct);
    }

    // Replaced LoadSchedulingRules with this version
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
