namespace AlatrafClinic.Application.Features.WoundedCards.Dtos;

public class WoundedCardDto
{
    public int WoundedCardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? CardImagePath { get; set; }
    public bool IsExpired { get; set; }
}