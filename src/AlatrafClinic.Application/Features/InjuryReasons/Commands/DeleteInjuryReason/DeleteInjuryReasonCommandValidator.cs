using FluentValidation;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.DeleteInjuryReason;
public class DeleteInjuryReasonCommandValidator : AbstractValidator<DeleteInjuryReasonCommand>
{
    public DeleteInjuryReasonCommandValidator()
    {
        RuleFor(x=> x.InjuryReasonId)
            .GreaterThan(0).WithMessage("Injury reason ID must be greater than zero.");
    }
}