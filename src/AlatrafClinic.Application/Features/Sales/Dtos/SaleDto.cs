using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Sales.Enums;

namespace AlatrafClinic.Application.Features.Sales.Dtos;

public class SaleDto
{
    public int SaleId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = new();
    public SaleStatus SaleStatus { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Total { get; set; }
    public List<SaleItemDto> SaleItems { get; set; } = new();
}