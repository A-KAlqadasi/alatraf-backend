using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public sealed class PaymentCoreDto
{
    public int PaymentId { get; set; }
    public int TicketId { get; set; }
    public int DiagnosisId { get; set; }
    public PaymentReference PaymentReference { get; set; }
    public AccountKind? AccountKind { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? PaymentDate { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? Discount { get; set; }
    public decimal Residual { get; set; }
    public string? Notes { get; set; }
}
