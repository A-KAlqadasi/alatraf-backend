using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class TherapyCardFilterRequest
{
    public string? SearchTerm { get; set; }

    public bool? IsActive { get; set; }

    public TherapyCardType? TherapyCardType { get; set; }

    public TherapyCardStatus? TherapyCardStatus { get; set; }

    public DateOnly? ProgramStartFrom { get; set; }

    public DateOnly? ProgramStartTo { get; set; }

    public DateOnly? ProgramEndFrom { get; set; }

    public DateOnly? ProgramEndTo { get; set; }

    public int? DiagnosisId { get; set; }

    public int? PatientId { get; set; }
    
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "SortColumn contains invalid characters.")]
    public string SortColumn { get; set; } = "ProgramStartDate";
    
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "desc";
}
