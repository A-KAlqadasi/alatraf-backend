using System.Collections.Generic;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Dtos;

public class StoreWithItemsDto
{
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public List<StoreItemUnitDto> Items { get; set; } = new();
}
