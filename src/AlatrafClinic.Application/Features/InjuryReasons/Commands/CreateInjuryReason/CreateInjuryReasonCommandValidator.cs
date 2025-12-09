using AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;

using FluentValidation;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.CreateInjuryReason;

public class CreateInjuryReasonCommandValidator : AbstractValidator<CreateInjuryReasonCommand>
{
    public CreateInjuryReasonCommandValidator()
    {
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury reason name is required.")
            .MaximumLength(100).WithMessage("Injury reason name must not exceed 100 characters.");
    }
}