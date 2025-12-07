using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class TherapyCardFilterRequest
{
    public string? SearchTerm { get; set; }

    public bool? IsActive { get; set; }

    public TherapyCardType? TherapyCardType { get; set; }

    public TherapyCardStatus? TherapyCardStatus { get; set; }

    public DateTime? ProgramStartFrom { get; set; }

    public DateTime? ProgramStartTo { get; set; }

    public DateTime? ProgramEndFrom { get; set; }

    public DateTime? ProgramEndTo { get; set; }

    public int? DiagnosisId { get; set; }

    public int? PatientId { get; set; }

    public string SortColumn { get; set; } = "ProgramStartDate";

    public string SortDirection { get; set; } = "desc";
}
