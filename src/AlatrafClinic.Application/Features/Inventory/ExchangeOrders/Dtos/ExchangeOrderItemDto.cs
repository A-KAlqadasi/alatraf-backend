namespace AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;

public class ExchangeOrderItemDto
{
    public int Id { get; set; }
    public int ExchangeOrderId { get; set; }
    public int StoreItemUnitId { get; set; }
    public string? ItemName { get; set; }
    public string? UnitName { get; set; }
    public decimal Quantity { get; set; }
}
