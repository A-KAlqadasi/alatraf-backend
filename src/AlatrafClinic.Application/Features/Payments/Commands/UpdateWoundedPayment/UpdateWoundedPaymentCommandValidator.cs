using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateWoundedPayment;

public class UpdateWoundedPaymentCommandValidator : AbstractValidator<UpdateWoundedPaymentCommand>
{
    public UpdateWoundedPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("PaymentId must be greater than 0.");

        RuleFor(x => x.DiagnosisId)
            .GreaterThan(0).WithMessage("DiagnosisId must be greater than 0.");

        RuleFor(x => x.AccountId)
            .GreaterThan(0).WithMessage("AccountId must be greater than 0.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("TotalAmount must be greater than 0.");

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("CardNumber is required.");

        RuleFor(x => x.ReportNumber)
            .MaximumLength(20).WithMessage("ReportNumber cannot exceed 20 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}