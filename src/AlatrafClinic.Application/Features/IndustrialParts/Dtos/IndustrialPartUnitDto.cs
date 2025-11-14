namespace AlatrafClinic.Application.Features.IndustrialParts.Dtos;

public class IndustrialPartUnitDto
{
    public int IndustrialPartUnitId { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = default!;
    public decimal PricePerUnit { get; set; }
}