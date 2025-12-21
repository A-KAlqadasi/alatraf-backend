using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;

public class WoundedPayment :AuditableEntity<int>
{
    public string? ReportNumber { get; private set; }
    public string? Notes { get; private set; }

    public Payment Payment { get; set; } = default!;

    private WoundedPayment() { }

    private WoundedPayment(int paymentId, string? reportNumber, string? notes) : base(paymentId)
    {
        ReportNumber = reportNumber;
        Notes = notes;
    }

    public static Result<WoundedPayment> Create(int paymentId, decimal total, decimal? minPriceForReportNumber, string? reportNumber, string? notes = null)
    {

        if (paymentId <= 0)
        {
            return WoundedPaymentErrors.PaymentIdIsRequired;
        }


        if (string.IsNullOrWhiteSpace(reportNumber) && minPriceForReportNumber.HasValue && total >= minPriceForReportNumber.Value)
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        return new WoundedPayment(
            paymentId,
            reportNumber,
            notes);
    }
    
    public Result<Updated> Update(decimal total, decimal minPriceForReportNumber, string? reportNumber, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(reportNumber) && total >= minPriceForReportNumber)
        {
            return WoundedPaymentErrors.ReportNumberIsRequired;
        }

        ReportNumber = reportNumber;
        Notes = notes;
        
        return Result.Updated;
    }
}