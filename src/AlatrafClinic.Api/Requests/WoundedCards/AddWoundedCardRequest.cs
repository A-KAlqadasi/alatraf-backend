using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.WoundedCards;

public sealed class AddWoundedCardRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "PatientId must be greater than 0.")]
    public int PatientId { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string CardNumber { get; init; } = string.Empty;

    [Required]
    public DateOnly ExpirationDate { get; init; }

    [StringLength(500)]
    public string? CardImagePath { get; init; }
}