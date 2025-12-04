using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRawOrder;

public class CreateRawOrderCommandValidator : AbstractValidator<CreateRawOrderCommand>
{
    public CreateRawOrderCommandValidator()
    {
        RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("SectionId is required.");
    }
}
