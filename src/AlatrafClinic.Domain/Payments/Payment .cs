

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Payments.Enums;

namespace AlatrafClinic.Domain.Patients.Payments;

   public sealed class Payment : AuditableEntity<int>
{
    public decimal TotalAmount { get; private set; }
    public decimal? PaidAmount { get; private set; }
    public decimal? Discount { get; private set; }

    public int AccountId { get; private set; }

    public PaymentType Type { get; private set; }


    public decimal Residual =>
        Math.Max(0, TotalAmount - ((PaidAmount ?? 0) + (Discount ?? 0)));

    private Payment() { }

    private Payment(decimal total, decimal? paid, decimal? discount, int accountId, PaymentType type)
    {
        TotalAmount = total;
        PaidAmount = paid;
        Discount = discount;
        AccountId = accountId;
        Type = type;
    }

 
    public static Result<Payment> Create(decimal total, decimal? paid, decimal? discount, int accountId, PaymentType type)
    {
        if (total <= 0)
            return PaymentErrors.InvalidTotal;

        if (paid is < 0)
            return PaymentErrors.InvalidPaid;

        if (discount is < 0)
            return PaymentErrors.InvalidDiscount;

        if ((paid ?? 0) + (discount ?? 0) > total)
            return PaymentErrors.OverPayment;

        return new Payment(total, paid, discount, accountId, type);
    }

   
    public Result<Updated> UpdateAmounts(decimal total, decimal? paid, decimal? discount)
    {
        if (total <= 0)
            return PaymentErrors.InvalidTotal;

        if (paid is < 0)
            return PaymentErrors.InvalidPaid;

        if (discount is < 0)
            return PaymentErrors.InvalidDiscount;

        if ((paid ?? 0) + (discount ?? 0) > total)
            return PaymentErrors.OverPayment;

        TotalAmount = total;
        PaidAmount = paid;
        Discount = discount;

        return Result.Updated;
    }

// if the paid amount is added incrementally 
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
