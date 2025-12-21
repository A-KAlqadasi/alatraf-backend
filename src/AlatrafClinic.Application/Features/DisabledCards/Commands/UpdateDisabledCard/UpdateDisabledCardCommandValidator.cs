using AlatrafClinic.Domain.Common.Constants;

using FluentValidation;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.UpdateDisabledCard;

public class UpdateDisabledCardCommandValidator : AbstractValidator<UpdateDisabledCardCommand>
{
    public UpdateDisabledCardCommandValidator()
    {
        RuleFor(x => x.DisabledCardId)
            .GreaterThan(0).WithMessage("Disabled card Id is invalid");
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient Id is invalid");
        RuleFor(x => x.IssueDate)
            .LessThanOrEqualTo(AlatrafClinicConstants.TodayDate).WithMessage("Issue date cannot be in the future");
        RuleFor(x => x.DisabilityType)
            .NotEmpty().WithMessage("Disability type is required");
    }
}