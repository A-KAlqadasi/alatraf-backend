using FluentValidation;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.DeleteDisabledCard;

public class DeleteDisabledCardCommandValidator : AbstractValidator<DeleteDisabledCardCommand>
{
    public DeleteDisabledCardCommandValidator()
    {
        RuleFor(x => x.DisabledCardId)
            .GreaterThan(0).WithMessage("DisabledCardId must be greater than 0.");
    }
}