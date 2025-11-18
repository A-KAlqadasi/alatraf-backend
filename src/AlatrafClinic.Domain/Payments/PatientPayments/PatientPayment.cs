using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public class PatientPayment : AuditableEntity<int>
{
    public string CouponNumber { get; private set; } = default!;
    public int PaymentId { get; private set; }
    public Payment? Payment { get; set; }

    private PatientPayment() { }

    private PatientPayment(
        string couponNumber,
        int paymentId
    ): base(paymentId)
    {
        CouponNumber = couponNumber;
        PaymentId = paymentId;
    }
    public static Result<PatientPayment> Create(
        string couponNumber,
        int paymentId
    )
    {
        if (string.IsNullOrWhiteSpace(couponNumber))
        {
            return PatientPaymentErrors.CouponNumberIsRequired;
        }

        if (paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        return new PatientPayment(
            couponNumber,
            paymentId
        );
    }

    public Result<Updated> Update(
        string couponNumber,
        int paymentId
    )
    {
        if (string.IsNullOrWhiteSpace(couponNumber))
        {
            return PatientPaymentErrors.CouponNumberIsRequired;
        }

        if (paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        CouponNumber = couponNumber;
        PaymentId = paymentId;

        return Result.Updated;
    }
}