using AlatrafClinic.Application.Features.WoundedCards.Commands.UpdateWoundedCard;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdateWoundedCard;

public class UpdateWoundedCardCommandValidator : AbstractValidator<UpdateWoundedCardCommand>
{
    public UpdateWoundedCardCommandValidator()
    {
         RuleFor(x => x.WoundedCardId)
            .GreaterThan(0).WithMessage("Wounded card Id is invalid");
            
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient Id is invalid");
        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Card is Expired!");
    }
}