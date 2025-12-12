using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.RepairCards.Enums;


namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class RepairCardFilterRequest
{

    [MaxLength(200, ErrorMessage = "SearchTerm cannot exceed 200 characters.")]
    public string? SearchTerm { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsLate { get; set; }

    public RepairCardStatus? Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DiagnosisId must be a positive number.")]
    public int? DiagnosisId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PatientId must be a positive number.")]
    public int? PatientId { get; set; }

    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "SortColumn contains invalid characters.")]
    public string SortColumn { get; set; } = "repaircardid";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "desc";
}