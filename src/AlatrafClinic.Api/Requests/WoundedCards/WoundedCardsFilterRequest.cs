using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.WoundedCards;

public sealed class WoundedCardsFilterRequest
{
    [StringLength(200)]
    public string? SearchTerm { get; init; }

    public bool? IsExpired { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "PatientId must be greater than 0.")]
    public int? PatientId { get; init; }

    public DateOnly? IssueDateFrom { get; init; }
    public DateOnly? IssueDateTo { get; init; }

    public DateOnly? ExpirationFrom { get; init; }
    public DateOnly? ExpirationTo { get; init; }

    [StringLength(50)]
    public string SortColumn { get; init; } = "Expiration";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be either 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "desc";
}