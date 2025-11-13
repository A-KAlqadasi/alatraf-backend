using FluentValidation;

namespace AlatrafClinic.Application.Features.Services.Commands.UpdateService;

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("Service ID must be greater than zero.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required.")
            .MaximumLength(200).WithMessage("Service name must not exceed 200 characters.");
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than zero.");
    }
}