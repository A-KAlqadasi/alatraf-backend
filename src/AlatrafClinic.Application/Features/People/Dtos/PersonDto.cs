namespace AlatrafClinic.Application.Features.People.Dtos;

public class PersonDto
{
    public int PersonId { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public DateOnly? Birthdate { get; set; }
    public string? Phone { get; set; }
    public string? NationalNo { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? AutoRegistrationNumber { get; set; }
}
