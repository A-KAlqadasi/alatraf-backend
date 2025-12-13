using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class GetPaidTherapyCardsFilterRequest
{
    [StringLength(100)]
    public string? SearchTerm { get; set; }

    [StringLength(50)]
    public string SortColumn { get; set; } = "PaymentDate";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "asc";

}
