using FluentValidation;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServiceById;

public class GetServiceByIdValidator : AbstractValidator<GetServiceByIdQuery>
{
    public GetServiceByIdValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Service ID is required.")
            .GreaterThan(0).WithMessage("Service ID must be greater than zero.");
    }
}