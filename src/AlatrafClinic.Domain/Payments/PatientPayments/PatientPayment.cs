using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public class PatientPayment : AuditableEntity<int>
{
    public string VoucherNumber { get; private set; } = default!;
    public string? Notes { get; private set; }

    public Payment Payment { get; set; } = default!;

    private PatientPayment() { }

    private PatientPayment(
        string couponNumber,
        int paymentId,
        string? notes
    ) : base(paymentId)
    {
        VoucherNumber = couponNumber;
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
        string? notes = null
    )
    {
        if (string.IsNullOrWhiteSpace(voucherNumber))
        {
            return PatientPaymentErrors.VoucherNumberIsRequired;
        }

        VoucherNumber = voucherNumber;
        Notes = notes;

        return Result.Updated;
    }
}