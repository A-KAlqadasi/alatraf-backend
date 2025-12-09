using FluentValidation;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.UpdateInjuryReason;

public class UpdateInjuryReasonCommandValidator : AbstractValidator<UpdateInjuryReasonCommand>
{
    public UpdateInjuryReasonCommandValidator()
    {
        RuleFor(x=> x.InjuryReasonId)
            .GreaterThan(0).WithMessage("Injury reason ID must be greater than zero.");
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury reason name is required.")
            .MaximumLength(100).WithMessage("Injury reason name must not exceed 100 characters.");
    }
}