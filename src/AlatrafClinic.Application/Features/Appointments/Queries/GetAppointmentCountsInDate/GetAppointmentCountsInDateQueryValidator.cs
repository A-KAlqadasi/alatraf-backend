using FluentValidation;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentCountsInDate;

public class GetAppointmentCountsInDateQueryValidator : AbstractValidator<GetAppointmentCountsInDateQuery>
{
    public GetAppointmentCountsInDateQueryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");
    }
}