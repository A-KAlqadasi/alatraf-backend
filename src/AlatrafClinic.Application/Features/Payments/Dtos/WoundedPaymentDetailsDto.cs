namespace AlatrafClinic.Application.Features.Payments.Dtos;

public sealed class WoundedPaymentDetailsDto
{
    public PaymentCoreDto Payment { get; set; } = new();

    public int WoundedCardId { get; set; }
    public string? ReportNumber { get; set; }
    public string? Notes { get; set; }
}
