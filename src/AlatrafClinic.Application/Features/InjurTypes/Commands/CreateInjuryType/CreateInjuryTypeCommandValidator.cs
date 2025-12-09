using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;

public class CreateInjuryTypeCommandValidator : AbstractValidator<CreateInjuryTypeCommand>
{
    public CreateInjuryTypeCommandValidator()
    {
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury type name is required.")
            .MaximumLength(100).WithMessage("Injury type name must not exceed 100 characters.");
    }
}