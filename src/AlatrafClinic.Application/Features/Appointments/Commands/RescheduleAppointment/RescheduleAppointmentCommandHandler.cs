using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<Updated>>
{
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppointmentScheduleRules _rules;
    private readonly HolidayCalendar _holidays;

    public RescheduleAppointmentCommandHandler(ILogger<RescheduleAppointmentCommandHandler> logger, IUnitOfWork unitOfWork, AppointmentScheduleRules rules, HolidayCalendar holidays)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _rules = rules;
        _holidays = holidays;
    }
    public async Task<Result<Updated>> Handle(RescheduleAppointmentCommand command, CancellationToken ct)
    {
        Appointment? appointment = await _unitOfWork.Appointments.GetByIdAsync(command.AppointmentId, ct);
        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }
        var rescheduleResult = appointment.Reschedule(command.NewAttendDate, _rules, _holidays);

        if (rescheduleResult.IsError)
        {
            _logger.LogWarning("Failed to reschedule appointment with ID {AppointmentId}: {Error}", command.AppointmentId, rescheduleResult.TopError);
            return rescheduleResult.Errors;
        }
        
        await _unitOfWork.Appointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Updated;
    }
}