using AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;

using FluentValidation;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.UpdateInjuryType;

public class UpdateInjuryTypeCommandValidator : AbstractValidator<UpdateInjuryTypeCommand>
{
    public UpdateInjuryTypeCommandValidator()
    {
        RuleFor(x=> x.InjuryTypeId)
            .GreaterThan(0).WithMessage("Injury type ID must be greater than zero.");
        RuleFor(x=> x.Name)
            .NotEmpty().WithMessage("Injury type name is required.")
            .MaximumLength(100).WithMessage("Injury type name must not exceed 100 characters.");
    }
}