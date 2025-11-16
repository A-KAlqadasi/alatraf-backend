using AlatrafClinic.Domain.AccountTypes;
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
    public AccountType? AccountType { get; private set; }
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

    public Result<Updated> Pay(decimal paid, decimal discount, int accountId)
    {
        if (paid < 0)
        {
            return PaymentErrors.InvalidPaid;
        }

        if (discount < 0)
        {
            return PaymentErrors.InvalidDiscount;
        }

        if (accountId <= 0)
        {
            return PaymentErrors.InvalidAccountId;
        }

        if (TotalAmount > (paid + discount))
        {
            return PaymentErrors.PaidAmountLessThanTotal;
        }

        if ((paid + discount) > TotalAmount)
        {
            return PaymentErrors.OverPayment;
        }

        PaidAmount = paid;
        Discount = discount;
        AccountId = accountId;
        IsCompleted = true;
        return Result.Updated;
    }


    public Result<Updated> AddPaidAmount(decimal amount)
    {
        if (amount <= 0)
            return PaymentErrors.InvalidPaid;

        var currentPaid = PaidAmount ?? 0;
        var currentDiscount = Discount ?? 0;

        if (currentPaid + amount > TotalAmount - currentDiscount)
            return PaymentErrors.OverPayment;

        PaidAmount = currentPaid + amount;
        return Result.Updated;
    }
    
    public bool IsFullyPaid => Residual == 0;
}