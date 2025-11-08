namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class SaleItemDto
{
    public int SaleItemId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
}