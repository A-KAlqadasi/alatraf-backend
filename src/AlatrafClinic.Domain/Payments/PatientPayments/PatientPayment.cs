using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public class PatientPayment : AuditableEntity<int>
{
    public string VoucherNumber { get; private set; } = default!;
    public int PaymentId { get; private set; }
    public Payment? Payment { get; set; }
    public string? Notes { get; private set; }

    private PatientPayment() { }

    private PatientPayment(
        string couponNumber,
        int paymentId,
        string? notes
    ) : base(paymentId)
    {
        VoucherNumber = couponNumber;
        PaymentId = paymentId;
        Notes = notes;
    }
    public static Result<PatientPayment> Create(
        string voucherNumber,
        int paymentId,
        string? notes = null
    )
    {
        if (string.IsNullOrWhiteSpace(voucherNumber))
        {
            return PatientPaymentErrors.VoucherNumberIsRequired;
        }

        if (paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        return new PatientPayment(
            voucherNumber,
            paymentId,
            notes
        );
    }

    public Result<Updated> Update(
        string voucherNumber,
        int paymentId,
        string? notes = null
    )
    {
        if (string.IsNullOrWhiteSpace(voucherNumber))
        {
            return PatientPaymentErrors.VoucherNumberIsRequired;
        }

        if (paymentId <= 0)
        {
            return PatientPaymentErrors.PaymentIdIsRequired;
        }

        VoucherNumber = voucherNumber;
        PaymentId = paymentId;
        Notes = notes;

        return Result.Updated;
    }
}