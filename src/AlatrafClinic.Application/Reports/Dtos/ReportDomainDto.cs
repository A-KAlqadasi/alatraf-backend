namespace AlatrafClinic.Application.Reports.Dtos;

// Add to your DTOs folder
public class ReportDomainDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RootTable { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
