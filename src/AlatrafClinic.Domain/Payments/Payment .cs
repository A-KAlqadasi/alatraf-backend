using System;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.Events;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.Tickets;

namespace AlatrafClinic.Domain.Payments;

public sealed class Payment : AuditableEntity<int>
{
    public Guid? SagaId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal? PaidAmount { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; set; } = default!;
    public int TicketId { get; private set; }
    public Ticket Ticket { get; set; } = default!;
    public PaymentReference PaymentReference { get; private set; }
    public AccountKind? AccountKind { get; private set; }
    public bool IsCompleted { get; private set; } = false;
    public string? Notes { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    public PatientPayment? PatientPayment { get; private set; }
    public DisabledPayment? DisabledPayment { get; private set; }
    public WoundedPayment? WoundedPayment { get; private set; }


    public decimal Residual =>
        Math.Max(0, TotalAmount - ((PaidAmount ?? 0m) + (DiscountPercentage ?? 0m)));

    private Payment() { }

    private Payment(Guid? sagaId, int ticketId, int diagnosisId, decimal total, PaymentReference reference, string? notes = null)
    {
        SagaId = sagaId;
        TicketId = ticketId;
        DiagnosisId = diagnosisId;
        TotalAmount = total;
        PaymentReference = reference;
        IsCompleted = false;
    }

    public static Result<Payment> Create(int ticketId, int diagnosisId, decimal total, PaymentReference reference, string? notes = null)
        => Create(null, ticketId, diagnosisId, total, reference, notes);

    public static Result<Payment> Create(Guid? sagaId, int ticketId, int diagnosisId, decimal total, PaymentReference reference, string? notes = null)
    {
        if (ticketId <= 0) return PaymentErrors.InvalidTicketId;
        if (total <= 0) return PaymentErrors.InvalidTotal;
        if (!Enum.IsDefined(typeof(PaymentReference), reference)) return PaymentErrors.InvalidPaymentReference;

        return new Payment(sagaId, ticketId, diagnosisId, total, reference, notes);
    }

    public Result<Updated> UpdateCore(int ticketId, int diagnosisId, decimal total, PaymentReference reference, string? notes = null)
    {
        if (ticketId <= 0) return PaymentErrors.InvalidTicketId;
        if (diagnosisId <= 0) return PaymentErrors.InvalidDiagnosisId;
        if (total <= 0) return PaymentErrors.InvalidTotal;
        if (!Enum.IsDefined(typeof(PaymentReference), reference)) return PaymentErrors.InvalidPaymentReference;

        TicketId = ticketId;
        DiagnosisId = diagnosisId;
        TotalAmount = total;
        PaymentReference = reference;
        Notes = notes;
        return Result.Updated;
    }

    public Result<Updated> Pay(decimal? paid, decimal? discountPercentage)
    {
        if (paid != null && paid < 0m)
            return PaymentErrors.InvalidPaid;

        if (discountPercentage != null && (discountPercentage < 0m || discountPercentage > 100m))
            return PaymentErrors.InvalidDiscount;

        decimal discountAmount = 0m;

        if (discountPercentage != null)
        {
            discountAmount = TotalAmount * (discountPercentage.Value / 100m);
        }

        decimal totalCovered = (paid.HasValue ? paid.Value : 0) + discountAmount;

        if (AccountKind == Payments.AccountKind.Patient && totalCovered > TotalAmount)
            return PaymentErrors.OverPayment;

        if (AccountKind == Payments.AccountKind.Patient && totalCovered < TotalAmount)
            return PaymentErrors.UnderPayment;

        // Assign
        PaidAmount = paid;
        DiscountPercentage = discountPercentage;
        DiscountAmount = discountAmount;
        IsCompleted = true;
        PaymentDate = DateTime.Now;

        AddDomainEvent(
       new PaymentCompletedDomainEvent(
           Id,
           DiagnosisId
            )
        );
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
        AccountKind = Payments.AccountKind.Patient;

        return Result.Updated;
    }

    public Result<Updated> AssignDisabledPayment(DisabledPayment disabledPayment)
    {
        if (disabledPayment == null) return PaymentErrors.InvalidDisabledPayment;
        ClearPaymentType();
        DisabledPayment = disabledPayment;
        AccountKind = Payments.AccountKind.Disabled;
        return Result.Updated;
    }

    public Result<Updated> AssignWoundedPayment(WoundedPayment woundedPayment)
    {
        if (woundedPayment == null) return PaymentErrors.InvalidWoundedPayment;
        ClearPaymentType();
        WoundedPayment = woundedPayment;
        AccountKind = Payments.AccountKind.Wounded;
        return Result.Updated;
    }

    public bool IsFullyPaid => Residual == 0m;



}