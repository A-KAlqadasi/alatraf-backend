namespace AlatrafClinic.Application.Features.Payments.Dtos;

public sealed class DisabledPaymentDetailsDto
{
    public PaymentCoreDto Payment { get; set; } = new();

    public int DisabledCardId { get; set; }
    public string? Notes { get; set; }
}
