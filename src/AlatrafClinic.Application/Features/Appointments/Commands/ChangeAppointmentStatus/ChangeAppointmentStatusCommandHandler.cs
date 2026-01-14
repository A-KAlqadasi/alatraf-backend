
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ChangeAppointmentStatus;

public class ChangeAppointmentStatusCommandHandler : IRequestHandler<ChangeAppointmentStatusCommand, Result<Updated>>
{
    private readonly ILogger<ChangeAppointmentStatusCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public ChangeAppointmentStatusCommandHandler(ILogger<ChangeAppointmentStatusCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<Updated>> Handle(ChangeAppointmentStatusCommand command, CancellationToken ct)
    {
        Appointment? appointment = await _context.Appointments
        .Include(a=> a.Ticket)
        .FirstOrDefaultAsync(a=> a.Id ==command.AppointmentId, ct);

        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }
        Result<Updated> result;

        switch (command.NewStatus)
        {
            case AppointmentStatus.Attended:
                result = appointment.MarkAsAttended();
                break;
            case AppointmentStatus.Absent:
                result = appointment.MarkAsAbsent();
                break;
            case AppointmentStatus.Today:
                result = appointment.MarkAsToday();
                break;
            case AppointmentStatus.Scheduled:
                result = appointment.MarkAsScheduled();
                break;
            case AppointmentStatus.Cancelled:
                result = appointment.Cancel();
                break;
            default:
                _logger.LogError("Invalid status {NewStatus} provided for appointment ID {AppointmentId}.", command.NewStatus, command.AppointmentId);
                return AppointmentErrors.InvalidStateTransition(appointment.Status, command.NewStatus);
        }

        if (result.IsError)
        {
            _logger.LogError("Failed to change status of appointment ID {AppointmentId} to {NewStatus}. Error: {Error}", command.AppointmentId, command.NewStatus, result.Errors);
            return result.Errors;
        }

        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Successfully changed status of appointment ID {AppointmentId} to {NewStatus}.", command.AppointmentId, command.NewStatus);

        return Result.Updated;
    }
}