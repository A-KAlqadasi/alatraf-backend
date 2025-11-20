using AlatrafClinic.Domain.Payments;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public class PayPaymentCommandValidator : AbstractValidator<PayPaymentCommand>
{
    public PayPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).GreaterThan(0);
        RuleFor(x => x.AccountKind).IsInEnum();
        When(x => x.AccountKind == AccountKind.Patient, () =>
        {
            RuleFor(x => x.PaidAmount).NotNull().WithMessage("PaidAmount is required for patient payments.");
            RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).When(x => x.Discount.HasValue);
            RuleFor(x => x.PatientPayment).NotNull().WithMessage("Patient payment details are required.");
            RuleFor(x => x.PatientPayment!.VoucherNumber).NotEmpty();
        });

        When(x => x.AccountKind == AccountKind.Disabled, () =>
        {
            RuleFor(x => x.DisabledPayment).NotNull().WithMessage("Disabled payment details are required.");
            RuleFor(x => x.DisabledPayment!.DisabledCardId).GreaterThan(0);
            RuleFor(x => x.PaidAmount).Null().WithMessage("PaidAmount must be null for Disabled payments.");
            RuleFor(x => x.Discount).Null().WithMessage("Discount must be null for Disabled payments.");
        });

        When(x => x.AccountKind == AccountKind.Wounded, () =>
        {
            RuleFor(x => x.WoundedPayment).NotNull().WithMessage("Wounded payment details are required.");
            RuleFor(x => x.WoundedPayment!.WoundedCardId).GreaterThan(0);
            RuleFor(x => x.PaidAmount).Null().WithMessage("PaidAmount must be null for Wounded payments.");
            RuleFor(x => x.Discount).Null().WithMessage("Discount must be null for Wounded payments.");
        });

        When(x => x.AccountKind == AccountKind.Free, () =>
        {
            RuleFor(x => x.PaidAmount).Null().WithMessage("PaidAmount must be null for Free payments.");
            RuleFor(x => x.Discount).Null().WithMessage("Discount must be null for Free payments.");
        });
    }
}