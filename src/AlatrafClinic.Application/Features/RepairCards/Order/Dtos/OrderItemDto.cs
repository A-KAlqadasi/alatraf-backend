namespace AlatrafClinic.Application.Features.RepairCards.Dtos;

public class OrderItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
