using FluentValidation;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.AddDisabledCard;

public class AddDisabledCardCommandValidator : AbstractValidator<AddDisabledCardCommand>
{
    public AddDisabledCardCommandValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient Id is invalid");
        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Card is Expired!");
    }
}