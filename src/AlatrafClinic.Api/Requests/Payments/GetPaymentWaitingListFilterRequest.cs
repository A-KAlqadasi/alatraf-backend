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