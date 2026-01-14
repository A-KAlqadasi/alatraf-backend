using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Services;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Appointments.SchedulingRulesService;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public sealed class RescheduleAppointmentCommandHandler
    : IRequestHandler<RescheduleAppointmentCommand, Result<Updated>>
{
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly AppointmentSchedulingService _schedulingService;
    
    public RescheduleAppointmentCommandHandler(
        ILogger<RescheduleAppointmentCommandHandler> logger,
        IAppDbContext context,
        AppointmentSchedulingService schedulingService)
    {
        _logger = logger;
        _context = context;
        _schedulingService = schedulingService;
    }
    
    public async Task<Result<Updated>> Handle(
        RescheduleAppointmentCommand command, 
        CancellationToken ct)
    {
        // 1. Load appointment
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == command.AppointmentId, ct);
        
        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }
        
        // 2. Get scheduling rules and context
        var rules = await _schedulingService.GetSchedulingRulesAsync(ct);
        var lastAppointmentDate = await _schedulingService.GetLastAppointmentDateAsync(ct);
        
        var context = SchedulingContext.Create(
            AlatrafClinicConstants.TodayDate,
            lastAppointmentDate,
            rules);
        
        // 3. Find next available date (excluding current appointment from count)
        DateOnly newAppointmentDate;
        try
        {
            newAppointmentDate = await _schedulingService.GetNextValidDateAsync(
                context,
                async date => await _schedulingService.GetAppointmentCountForDateAsync(date, command.AppointmentId, ct));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "No available appointment dates found for rescheduling Appointment {AppointmentId}", command.AppointmentId);
            return Error.Failure("Appointment.NoAvailableDates", ex.Message);
        }
        
        // 4. Reschedule
        var rescheduleResult = appointment.Reschedule(newAppointmentDate);
        
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
}