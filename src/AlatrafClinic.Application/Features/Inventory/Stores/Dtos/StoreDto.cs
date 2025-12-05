namespace AlatrafClinic.Application.Features.Inventory.Stores.Dtos;

public class StoreDto
{
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
}
