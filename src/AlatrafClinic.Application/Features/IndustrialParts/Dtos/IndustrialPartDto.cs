namespace AlatrafClinic.Application.Features.IndustrialParts.Dtos;

public class IndustrialPartDto
{
    public int IndustrialPartId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<IndustrialPartUnitDto> IndustrialPartUnits { get; set; } = new();
}
