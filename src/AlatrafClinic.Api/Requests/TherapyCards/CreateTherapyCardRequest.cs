using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class CreateTherapyCardRequest
{
    [Required(ErrorMessage = "TicketId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "TicketId must be greater than 0.")]
    public int TicketId { get; set; }

    [Required(ErrorMessage = "DiagnosisText is required.")]
    [MaxLength(1000, ErrorMessage = "DiagnosisText must not exceed 1000 characters.")]
    public string DiagnosisText { get; set; } = default!;

    [Required(ErrorMessage = "InjuryDate is required.")]
    [DataType(DataType.Date)]
    public DateOnly InjuryDate { get; set; }

    [Required(ErrorMessage = "InjuryReasons is required.")]
    public List<int> InjuryReasons { get; set; } = new();

    [Required(ErrorMessage = "InjurySides is required.")]
    public List<int> InjurySides { get; set; } = new();

    [Required(ErrorMessage = "InjuryTypes is required.")]
    public List<int> InjuryTypes { get; set; } = new();

    [Required(ErrorMessage = "ProgramStartDate is required.")]
    [DataType(DataType.Date)]
    public DateOnly ProgramStartDate { get; set; }

    [Required(ErrorMessage = "ProgramEndDate is required.")]
    [DataType(DataType.Date)]
    public DateOnly ProgramEndDate { get; set; }

    [Required(ErrorMessage = "TherapyCardType is required.")]
    [EnumDataType(typeof(TherapyCardType), ErrorMessage = "Invalid TherapyCardType.")]
    public TherapyCardType TherapyCardType { get; set; }

    [Required(ErrorMessage = "Programs list is required.")]
    public List<TherapyCardMedicalProgramRequest> Programs { get; set; } = new();

    public string? Notes { get; set; }
}
