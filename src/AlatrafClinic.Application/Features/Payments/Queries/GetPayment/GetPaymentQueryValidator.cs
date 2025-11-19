using FluentValidation;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentQueryValidator : AbstractValidator<GetPaymentQuery>
{
    public GetPaymentQueryValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("PaymentId is invalid.");
    }
}