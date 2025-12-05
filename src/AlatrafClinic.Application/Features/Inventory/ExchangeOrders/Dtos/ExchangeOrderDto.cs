using System.Collections.Generic;

namespace AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;

public class ExchangeOrderDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? Notes { get; set; }
    public int? SaleId { get; set; }
    public int? OrderId { get; set; }
    public List<ExchangeOrderItemDto>? Items { get; set; }
}
