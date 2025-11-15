using FluentValidation;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public class UpdateIndustrialPartCommandValidator : AbstractValidator<UpdateIndustrialPartCommand>
{
    public UpdateIndustrialPartCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Industrial part name is required.")
            .MaximumLength(200).WithMessage("Industrial part name must not exceed 200 characters.");
        
        RuleForEach(x => x.Units)
            .SetValidator(new UpdateIndustrialPartUnitCommandValidator());
    }
}