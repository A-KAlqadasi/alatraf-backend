using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public sealed class PaymentListItemDto
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

    // Helpful for list UI: show "identifier" per type
    public string? VoucherNumber { get; set; }
    public int? DisabledCardId { get; set; }
    public int? WoundedCardId { get; set; }
}
