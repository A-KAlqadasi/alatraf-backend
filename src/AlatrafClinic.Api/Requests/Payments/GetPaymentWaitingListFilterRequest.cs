using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Api.Requests.Payments;

public sealed class GetPaymentsWaitingListFilterRequest
{
    public string? SearchTerm { get; set; }
    public PaymentReference? PaymentReference { get; set; }
    public bool? IsCompleted { get; set; }
    
    // [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "SortColumn contains invalid characters.")]
    public string SortColumn {get; set; } = "CreatedAtUtc";
    // [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "desc";
}

public sealed class PayFreePaymentRequest
{
    // No body fields required for free payment
}

public sealed class PayPatientPaymentRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "PaidAmount must be greater than 0.")]
    public decimal PaidAmount { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Discount { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string VoucherNumber { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; init; }
}

public sealed class PayDisabledPaymentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "DisabledCardId must be greater than 0.")]
    public int DisabledCardId { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }
}

public sealed class PayWoundedPaymentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "WoundedCardId must be greater than 0.")]
    public int WoundedCardId { get; init; }

    [StringLength(50)]
    public string? ReportNumber { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }
}