using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateDisabledPayment;

public class UpdateDisabledPaymentCommandValidator : AbstractValidator<UpdateDisabledPaymentCommand>
{
    public UpdateDisabledPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithMessage("Payment ID must be greater than 0.");
        RuleFor(x => x.DiagnosisId)
            .GreaterThan(0)
            .WithMessage("Diagnosis ID must be greater than 0.");
        RuleFor(x => x.AccountId)
            .GreaterThan(0)
            .WithMessage("Account ID must be greater than 0.");
            
        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total amount must be greater than or equal to 0.");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number is required.")
            .MaximumLength(16)
            .WithMessage("Card number must not exceed 16 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters.");
    }
}