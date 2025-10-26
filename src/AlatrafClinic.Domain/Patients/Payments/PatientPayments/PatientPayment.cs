using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Payments.PatientPayments;

public class PatientPayment : AuditableEntity<int>
{
    public string? CouponNumber { get; set; }
    public int? PaymentId { get; set; }
    //public Payment? Payment { get; set; }

    private PatientPayment() { }

    private PatientPayment(
        string? couponNumber,
        int? paymentId
    )
    {
        CouponNumber = couponNumber;
        PaymentId = paymentId;
    }
    public static Result<PatientPayment> Create(
        string? couponNumber,
        int? paymentId
    )
    {
        if (string.IsNullOrWhiteSpace(couponNumber))
        {
            return PatientPaymentErrors.CouponNumberIsRequired;
        }

        if (paymentId == null || paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        return new PatientPayment(
            couponNumber,
            paymentId
        );
    }

    public Result<Updated> Update(
        string? couponNumber,
        int? paymentId
    )
    {
        if (string.IsNullOrWhiteSpace(couponNumber))
        {
            return PatientPaymentErrors.CouponNumberIsRequired;
        }

        if (paymentId == null || paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        CouponNumber = couponNumber;
        PaymentId = paymentId;

        return Result.Updated;
    }
}