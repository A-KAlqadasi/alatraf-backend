using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Domain.Payments;

public sealed class Payment : AuditableEntity<int>
{
    public decimal TotalAmount { get; private set; }
    public decimal? PaidAmount { get; private set; }
    public decimal? Discount { get; private set; }
    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; set; } = default!;
    public int TicketId {get; private set;}
    public Ticket Ticket { get; set; } = default!;
    public PaymentReference PaymentReference { get; private set; }
    public AccountKind AccountKind { get; private set; }
    public bool IsCompleted { get; private set; } = false;

    public PatientPayment? PatientPayment { get; private set; }
    public DisabledPayment? DisabledPayment { get; private set; }
    public WoundedPayment? WoundedPayment { get; private set; }

    public decimal Residual =>
        Math.Max(0, TotalAmount - ((PaidAmount ?? 0m) + (Discount ?? 0m)));

    private Payment() { }

    private Payment(int ticketId, int diagnosisId, decimal total, PaymentReference reference)
    {
        TicketId = ticketId;
        DiagnosisId = diagnosisId;
        TotalAmount = total;
        PaymentReference = reference;
        IsCompleted = false;
    }

    public static Result<Payment> Create(int ticketId, int diagnosisId, decimal total, PaymentReference reference)
    {
        if (ticketId <= 0) return PaymentErrors.InvalidTicketId;
        if (diagnosisId <= 0) return PaymentErrors.InvalidDiagnosisId;
        if (total <= 0) return PaymentErrors.InvalidTotal;
        if (!Enum.IsDefined(typeof(PaymentReference), reference)) return PaymentErrors.InvalidPaymentReference;

        return new Payment(ticketId, diagnosisId, total, reference);
    }

    public Result<Updated> UpdateCore(int ticketId, int diagnosisId, decimal total, PaymentReference reference)
    {
        if (ticketId <= 0) return PaymentErrors.InvalidTicketId;
        if (diagnosisId <= 0) return PaymentErrors.InvalidDiagnosisId;
        if (total <= 0) return PaymentErrors.InvalidTotal;
        if (!Enum.IsDefined(typeof(PaymentReference), reference)) return PaymentErrors.InvalidPaymentReference;
        
        TicketId = ticketId;
        DiagnosisId = diagnosisId;
        TotalAmount = total;
        PaymentReference = reference;
        return Result.Updated;
    }

    public Result<Updated> Pay(decimal? paid, decimal? discount)
    {
        // paid and discount must be non-negative if provided
        if (paid != null && paid < 0m) return PaymentErrors.InvalidPaid;
        if (discount != null && discount < 0m) return PaymentErrors.InvalidDiscount;

        // Overpayment check
        if (((paid ?? 0m) + (discount ?? 0m)) > TotalAmount)
            return PaymentErrors.OverPayment;

        // Assign
        PaidAmount = paid;
        Discount = discount;

        IsCompleted = true;

        return Result.Updated;
    }

    public void ClearPaymentType()
    {
        PatientPayment = null;
        DisabledPayment = null;
        WoundedPayment = null;
    }

    public void MarkAccountKind(AccountKind kind)
    {
        AccountKind = kind;
    }

    public Result<Updated> AssignPatientPayment(PatientPayment patientPayment)
    {
        if (patientPayment == null) return PaymentErrors.InvalidPatientPayment;
        // Clear other types to ensure single-type invariant
        ClearPaymentType();
        PatientPayment = patientPayment;
        AccountKind = AccountKind.Patient;
        return Result.Updated;
    }

    public Result<Updated> AssignDisabledPayment(DisabledPayment disabledPayment)
    {
        if (disabledPayment == null) return PaymentErrors.InvalidDisabledPayment;
        ClearPaymentType();
        DisabledPayment = disabledPayment;
        AccountKind = AccountKind.Disabled;
        return Result.Updated;
    }

    public Result<Updated> AssignWoundedPayment(WoundedPayment woundedPayment)
    {
        if (woundedPayment == null) return PaymentErrors.InvalidWoundedPayment;
        ClearPaymentType();
        WoundedPayment = woundedPayment;
        AccountKind = AccountKind.Wounded;
        return Result.Updated;
    }

    public bool IsFullyPaid => Residual == 0m;
}