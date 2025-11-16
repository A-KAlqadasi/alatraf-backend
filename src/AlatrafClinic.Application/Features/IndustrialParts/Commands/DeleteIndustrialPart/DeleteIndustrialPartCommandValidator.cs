using FluentValidation;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.DeleteIndustrialPart;

public class DeleteIndustrialPartCommandValidator : AbstractValidator<DeleteIndustrialPartCommand>
{
    public DeleteIndustrialPartCommandValidator()
    {
        RuleFor(x => x.IndustrialPartId)
            .GreaterThan(0).WithMessage("Industrial Part Id must be greater than zero.");
    }
}