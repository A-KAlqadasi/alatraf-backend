namespace AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;

public class PurchaseItemDto
{
    public int Id { get; set; }
    public int StoreItemUnitId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
}
