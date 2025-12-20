using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class UpdateTherapyCardRequest
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
    
    public DateOnly? ProgramEndDate { get; set; }

    [Required(ErrorMessage = "Number of sessions is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of sessions must be greater than 0.")]
    public int NumberOfSessions { get; set; }

    [Required(ErrorMessage = "TherapyCardType is required.")]
    [EnumDataType(typeof(TherapyCardType), ErrorMessage = "Invalid TherapyCardType.")]
    public TherapyCardType TherapyCardType { get; set; }

    [MinLength(1, ErrorMessage = "At least one program must be provided.")]
    public List<TherapyCardMedicalProgramRequest> Programs { get; set; } =[];

    public string? Notes { get; set; }
}
