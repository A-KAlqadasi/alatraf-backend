using AlatrafClinic.Domain.Payments;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetRepairPayment;

public class GetRepairPaymentQueryValidator : AbstractValidator<GetRepairPaymentQuery>
{
    public GetRepairPaymentQueryValidator()
    {
        RuleFor(x=> x.paymentId)
            .GreaterThan(0).WithMessage("Payment id must be greater than 0.");
        RuleFor(x=> x.PaymentReference)
            .Equal(PaymentReference.Repair).WithMessage("Payment referenc must be repair");
    }
}