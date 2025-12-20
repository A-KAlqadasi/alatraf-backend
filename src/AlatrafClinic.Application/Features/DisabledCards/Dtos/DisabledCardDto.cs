namespace AlatrafClinic.Application.Features.DisabledCards.Dtos;

public class DisabledCardDto
{
    public int DisabledCardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public DateOnly IssueDate { get; set; }
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;    
    public string? CardImagePath { get; set; }
    public bool IsExpired { get; set; }
}
