using FluentValidation;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
{
    public GetAppointmentByIdQueryValidator()
    {
        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("AppointmentId must be greater than zero.");
    }
}