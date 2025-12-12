using AlatrafClinic.Domain.Common.Constants;

using FluentValidation;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.AddWoundedCard;

public class AddWoundedCardCommandValidator : AbstractValidator<AddWoundedCardCommand>
{
    public AddWoundedCardCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("CardNumber is required.")
            .MaximumLength(50).WithMessage("CardNumber must not exceed 50 characters.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(AlatrafClinicConstants.TodayDate).WithMessage("ExpirationDate must be a future date.");
    }
}