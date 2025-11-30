using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;

public class WoundedPayment :AuditableEntity<int>
{
    public int WoundedCardId { get; private set; }
    public WoundedCard? WoundedCard { get; private set; }
    public string? ReportNumber { get; private set; }
    public string? Notes { get; private set; }

    public Payment Payment { get; set; } = default!;

    private WoundedPayment() { }

    private WoundedPayment(int paymentId, int woundedCardId, string? reportNumber, string? notes) : base(paymentId)
    {
        WoundedCardId = woundedCardId;
        ReportNumber = reportNumber;
        Notes = notes;
    }

    public static Result<WoundedPayment> Create(int paymentId, decimal total, decimal minPriceForReportNumber, int woundedCardId, string? reportNumber, string? notes = null)
    {

        if (paymentId <= 0)
        {
            return WoundedPaymentErrors.PaymentIdIsRequired;
        }

        if (woundedCardId <= 0)
        {
            return WoundedPaymentErrors.WoundedCardIdIsRequired;
        }

        if (string.IsNullOrWhiteSpace(reportNumber) && total >= minPriceForReportNumber)
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        return new WoundedPayment(
            paymentId,
            woundedCardId,
            reportNumber,
            notes);
    }
    
    public Result<Updated> Update(int woundedCardId, decimal total, decimal minPriceForReportNumber, string? reportNumber, string? notes = null)
    {

        if (woundedCardId <= 0)
        {
            return WoundedPaymentErrors.WoundedCardIdIsRequired;
        }

        if (string.IsNullOrWhiteSpace(reportNumber) && total >= minPriceForReportNumber)
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        WoundedCardId = woundedCardId;
        ReportNumber = reportNumber;
        Notes = notes;
        
        return Result.Updated;
    }
}