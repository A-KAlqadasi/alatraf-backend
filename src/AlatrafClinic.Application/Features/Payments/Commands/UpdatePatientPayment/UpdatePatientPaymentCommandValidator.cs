using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdatePatientPayment;

public class UpdatePatientPaymentCommandValidator : AbstractValidator<UpdatePatientPaymentCommand>
{
    public UpdatePatientPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("PaymentId must be greater than 0.");
        RuleFor(x => x.DiagnosisId)
            .GreaterThan(0).WithMessage("DiagnosisId must be greater than 0.");

        RuleFor(x => x.AccountId)
            .GreaterThan(0).WithMessage("AccountId must be greater than 0.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("TotalAmount must be greater than 0.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("PaidAmount must be greater than or equal to 0.")
            .LessThanOrEqualTo(x => x.TotalAmount).WithMessage("PaidAmount cannot exceed TotalAmount.");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("DiscountAmount must be greater than or equal to 0.")
            .LessThanOrEqualTo(x => x.TotalAmount).WithMessage("DiscountAmount cannot exceed TotalAmount.");

        RuleFor(x => x.VoucherNumber)
            .NotEmpty().WithMessage("VoucherNumber is required.")
            .MaximumLength(50).WithMessage("VoucherNumber cannot exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}