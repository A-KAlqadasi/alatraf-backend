namespace AlatrafClinic.Application.Features.Payments.Dtos;

public sealed class PatientPaymentDetailsDto
{
    public PaymentCoreDto Payment { get; set; } = new();

    public string VoucherNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
