using AlatrafClinic.Domain.Payments;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetTherapyPayment;

public class GetTherapyPaymentQueryValidator : AbstractValidator<GetTherapyPaymentQuery>
{
    public GetTherapyPaymentQueryValidator()
    {
        RuleFor(x=> x.paymentId)
            .GreaterThan(0).WithMessage("Payment id must be greater than 0.");
        RuleFor(x=> x.PaymentReference)
            .NotEqual(PaymentReference.Repair).WithMessage("Payment referenc must be therapy related")
            .NotEqual(PaymentReference.Sales).WithMessage("Payment referenc must be therapy related");
    }
}