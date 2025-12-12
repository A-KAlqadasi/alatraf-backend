using AlatrafClinic.Domain.Common.Constants;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("Appointment ID must be greater than zero.");

        RuleFor(x => x.NewAttendDate)
            .GreaterThan(AlatrafClinicConstants.TodayDate).WithMessage("New attend date must be in the future.");
    }
}