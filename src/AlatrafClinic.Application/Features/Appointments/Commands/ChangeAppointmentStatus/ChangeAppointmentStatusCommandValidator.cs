using FluentValidation;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ChangeAppointmentStatus;

public class ChangeAppointmentStatusCommandValidator : AbstractValidator<ChangeAppointmentStatusCommand>
{
    public ChangeAppointmentStatusCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("Appointment ID must be greater than zero.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("New status must be a valid appointment status.");
    }
}