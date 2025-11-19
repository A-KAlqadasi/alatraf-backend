using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;

namespace AlatrafClinic.Domain.Payments;

public sealed class Payment : AuditableEntity<int>
{
    public decimal TotalAmount { get; private set; }
    public decimal? PaidAmount { get; private set; }
    public decimal? Discount { get; private set; }
    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; set; } = default!;
    
    public int? AccountId { get; private set; }
    public Account? Account { get; private set; }
    public PaymentType Type { get; private set; }
    public bool IsCompleted { get; private set; } = false;

    public PatientPayment? PatientPayment { get; private set; }
    public DisabledPayment? DisabledPayment { get; private set; }
    public WoundedPayment? WoundedPayment { get; private set; }

    public decimal Residual =>
        Math.Max(0, TotalAmount - ((PaidAmount ?? 0) + (Discount ?? 0)));

    private Payment() { }

    private Payment(int diagnosisId, decimal total, PaymentType type)
    {
        DiagnosisId = diagnosisId;
        TotalAmount = total;
        Type = type;
        IsCompleted = false;
    }

    public static Result<Payment> Create(int diagnosisId, decimal total, PaymentType type)
    {

        if (total <= 0) { return PaymentErrors.InvalidTotal; }

        if (!Enum.IsDefined(typeof(PaymentType), type))
        {
            return PaymentErrors.InvalidPaymentType;
        }

        return new Payment(diagnosisId, total, type);
    }
    
    public Result<Updated> Update(int diagnosisId, decimal total, PaymentType type)
    {
        if (diagnosisId <= 0)
        {
            return PaymentErrors.InvalidDiagnosisId;
        }
        
        if (total <= 0)
        {
            return PaymentErrors.InvalidTotal;
        }

        if (!Enum.IsDefined(typeof(PaymentType), type))
        {
            return PaymentErrors.InvalidPaymentType;
        }

        DiagnosisId = diagnosisId;
        TotalAmount = total;
        Type = type;

        return Result.Updated;
    }

    public Result<Updated> Pay(decimal total, decimal? paid, decimal? discount, int accountId)
    {
        if (total != TotalAmount)
        {
            return PaymentErrors.TotalMissmatch;
        }
        
        if (paid != null && paid < 0)
        {
            return PaymentErrors.InvalidPaid;
        }

        if (discount != null && discount < 0)
        {
            return PaymentErrors.InvalidDiscount;
        }

        if (accountId <= 0)
        {
            return PaymentErrors.InvalidAccountId;
        }

        if ((paid ?? 0 + discount ?? 0) > TotalAmount)
        {
            return PaymentErrors.OverPayment;
        }

        PaidAmount = paid;
        Discount = discount;
        AccountId = accountId;
        IsCompleted = true;
        return Result.Updated;
    }

    public Result<Updated> AssignPatientPayment(PatientPayment patientPayment)
    {
        if (patientPayment == null)
        {
            return PaymentErrors.InvalidPatientPayment;
        }

        PatientPayment = patientPayment;
        return Result.Updated;
    }
    public Result<Updated> AssignDisabledPayment(DisabledPayment disabledPayment)
    {
        if (disabledPayment == null)
        {
            return PaymentErrors.InvalidDisabledPayment;
        }

        DisabledPayment = disabledPayment;
        return Result.Updated;
    }

    public Result<Updated> AssignWoundedPayment(WoundedPayment woundedPayment)
    {
        if (woundedPayment == null)
        {
            return PaymentErrors.InvalidWoundedPayment;
        }

        WoundedPayment = woundedPayment;
        return Result.Updated;
    }
    
    public bool IsFullyPaid => Residual == 0;
}