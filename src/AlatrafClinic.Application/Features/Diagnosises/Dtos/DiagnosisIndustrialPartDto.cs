namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class DiagnosisIndustrialPartDto
{
    public int DiagnosisIndustrialPartId { get; set; }
    public int IndustrialPartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}