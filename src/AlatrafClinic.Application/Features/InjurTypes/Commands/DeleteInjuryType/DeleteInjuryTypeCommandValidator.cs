using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.DeleteInjuryType;

public class DeleteInjuryTypeCommandValidator : AbstractValidator<DeleteInjuryTypeCommand>
{
    public DeleteInjuryTypeCommandValidator()
    {
        RuleFor(x=> x.InjuryTypeId)
            .GreaterThan(0).WithMessage("Injury type ID must be greater than zero.");
    }
}