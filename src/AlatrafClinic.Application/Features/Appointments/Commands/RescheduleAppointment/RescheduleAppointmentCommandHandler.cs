using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<Updated>>
{
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public RescheduleAppointmentCommandHandler(ILogger<RescheduleAppointmentCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(RescheduleAppointmentCommand command, CancellationToken ct)
    {
        Appointment? appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id ==command.AppointmentId, ct);
        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }

        var lastAppointment = await _context.Appointments.OrderByDescending(a=> a.AttendDate).FirstOrDefaultAsync(ct);

        DateTime lastAppointmentDate = lastAppointment?.AttendDate ?? DateTime.MinValue;

        DateTime baseDate = lastAppointmentDate.Date < DateTime.Now.Date ? DateTime.Now.Date : lastAppointmentDate.Date;

        if (command.NewAttendDate.Date > baseDate)
        {
            baseDate = command.NewAttendDate.Date;
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

        var rescheduleResult = appointment.Reschedule(baseDate);

        if (rescheduleResult.IsError)
        {
            _logger.LogWarning("Failed to reschedule appointment with ID {AppointmentId}: {Error}", command.AppointmentId, rescheduleResult.TopError);
            return rescheduleResult.Errors;
        }
        
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("appointment", ct);

        _logger.LogInformation("Appointment with Id {appointmentId}, rescheduled to {newDate}", appointment.Id, appointment.AttendDate);

        return Result.Updated;
    }
}