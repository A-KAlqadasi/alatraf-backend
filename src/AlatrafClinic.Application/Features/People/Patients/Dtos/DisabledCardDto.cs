namespace AlatrafClinic.Application.Features.People.Patients.Dtos;

public class DisabledCardDto
{
    public int DisabledCardId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? CardImagePath { get; set; }
    public bool IsExpired { get; set; }

}