using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.WoundedCards;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;

public class WoundedPayment :AuditableEntity<int>
{
    public int? WoundedCardId { get; private set; }
    public WoundedCard? WoundedCard { get; private set; }

    public decimal Total { get; private set; }
    public decimal MinimumPriceForReportNumber { get; private set; } = 30000;

    public int PaymentId { get; private set; }
    public Payment? Payment { get; private set; }
    public string? ReportNumber { get; private set; }

    private WoundedPayment() { }

    private WoundedPayment(int paymentId, decimal total, decimal minimumPriceForReportNumber, int? woundedCardId, string? reportNumber)
    {
        WoundedCardId = woundedCardId;
        PaymentId = paymentId;
        Total = total;
        MinimumPriceForReportNumber = minimumPriceForReportNumber;
        ReportNumber = reportNumber;
    }

    public static Result<WoundedPayment> Create(int paymentId, decimal total, decimal minimumPriceForReportNumber, int? woundedCardId, string? reportNumber)
    {
        

        if (paymentId <= 0)
        {
            return WoundedPaymentErrors.PaymentIdIsRequired;
        }

        if (total <= 0)
        {
            return WoundedPaymentErrors.TotalIsRequired;
        }

        if (minimumPriceForReportNumber <= 0)
        {
            return WoundedPaymentErrors.MinimumPriceForReportNumberIsRequired;
        }

        if (woundedCardId is null || woundedCardId <= 0)
        {
            return WoundedPaymentErrors.WoundedCardIdIsRequired;
        }

        if (total >= minimumPriceForReportNumber && string.IsNullOrWhiteSpace(reportNumber))
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        return new WoundedPayment(
            paymentId,
            total,
            minimumPriceForReportNumber,
            woundedCardId,
            reportNumber);
    }
    
    public Result<Updated> Update(int paymentId, decimal total, decimal minimumPriceForReportNumber, int? woundedCardId, string? reportNumber)
    {
        
        if (paymentId <= 0)
        {
            return WoundedPaymentErrors.PaymentIdIsRequired;
        }

        if (total <= 0)
        {
            return WoundedPaymentErrors.TotalIsRequired;
        }

        if (minimumPriceForReportNumber <= 0)
        {
            return WoundedPaymentErrors.MinimumPriceForReportNumberIsRequired;
        }

        if (woundedCardId is null || woundedCardId <= 0)
        {
            return WoundedPaymentErrors.WoundedCardIdIsRequired;
        }

        if (total >= minimumPriceForReportNumber && string.IsNullOrWhiteSpace(reportNumber))
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        WoundedCardId = woundedCardId;
        PaymentId = paymentId;
        Total = total;
        MinimumPriceForReportNumber = minimumPriceForReportNumber;
        ReportNumber = reportNumber;
        
        return Result.Updated;
    }
}