using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Api.Requests.Payments;

public sealed class PaymentsFilterRequest
{
    [StringLength(200)]
    public string? SearchTerm { get; init; }

    [Range(1, int.MaxValue)]
    public int? TicketId { get; init; }

    [Range(1, int.MaxValue)]
    public int? DiagnosisId { get; init; }

    public PaymentReference? PaymentReference { get; init; }

    public AccountKind? AccountKind { get; init; }

    public bool? IsCompleted { get; init; }

    public DateTime? PaymentDateFrom { get; init; }

    public DateTime? PaymentDateTo { get; init; }

    [StringLength(50)]
    public string SortColumn { get; init; } = "PaymentDate";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be either 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "desc";
}
