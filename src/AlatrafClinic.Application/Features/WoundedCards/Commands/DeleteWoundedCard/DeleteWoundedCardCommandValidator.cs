using FluentValidation;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.DeleteWoundedCard;

public class DeleteWoundedCardCommandValidator : AbstractValidator<DeleteWoundedCardCommand>
{
    public DeleteWoundedCardCommandValidator()
    {
        RuleFor(x => x.WoundedCardId)
            .GreaterThan(0).WithMessage("WoundedCardId must be greater than 0.");
    }
}