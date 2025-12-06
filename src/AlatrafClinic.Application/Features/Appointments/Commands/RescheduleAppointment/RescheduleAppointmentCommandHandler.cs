using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<Updated>>
{
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleAppointmentCommandHandler(ILogger<RescheduleAppointmentCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Updated>> Handle(RescheduleAppointmentCommand command, CancellationToken ct)
    {
        Appointment? appointment = await _unitOfWork.Appointments.GetByIdAsync(command.AppointmentId, ct);
        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", command.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }

        DateTime lastAppointmentDate = await _unitOfWork.Appointments.GetLastAppointmentAttendDate(ct);

        DateTime baseDate = lastAppointmentDate.Date < DateTime.Now.Date ? DateTime.Now.Date : lastAppointmentDate.Date;

        if (command.NewAttendDate.Date > baseDate)
        {
            baseDate = command.NewAttendDate.Date;
        }

        var allowedDaysString = await _unitOfWork.AppSettings.GetAllowedAppointmentDaysAsync(ct);
        
        var allowedDays = allowedDaysString.Split(',').Select(day => Enum.Parse<DayOfWeek>(day.Trim())).ToList();

        var holidays = await _unitOfWork.Holidays.GetAllAsync(ct);


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
        
        await _unitOfWork.Appointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Updated;
    }
}