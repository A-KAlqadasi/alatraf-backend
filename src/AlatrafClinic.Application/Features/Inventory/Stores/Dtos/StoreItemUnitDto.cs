namespace AlatrafClinic.Application.Features.Inventory.Stores.Dtos;

public class StoreItemUnitDto
{
    public int StoreItemUnitId { get; set; }
    public int StoreId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    
}
