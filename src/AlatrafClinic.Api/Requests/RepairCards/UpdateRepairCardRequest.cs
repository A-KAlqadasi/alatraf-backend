using System.ComponentModel.DataAnnotations;


namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class UpdateRepairCardRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TicketId must be a positive number.")]
    public int TicketId { get; set; }

    [Required]
    [StringLength(2000, ErrorMessage = "DiagnosisText cannot exceed 2000 characters.")]
    public string DiagnosisText { get; set; } = default!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime InjuryDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one InjuryReason is required.")]
    public List<int> InjuryReasons { get; set; } = new();

    [Required]
    [MinLength(1, ErrorMessage = "At least one InjurySide is required.")]
    public List<int> InjurySides { get; set; } = new();

    [Required]
    [MinLength(1, ErrorMessage = "At least one InjuryType is required.")]
    public List<int> InjuryTypes { get; set; } = new();

    [Required]
    [MinLength(1, ErrorMessage = "At least one industrial part is required.")]
    public List<RepairCardIndustrialPartRequest> IndustrialParts { get; set; } = new();

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
}