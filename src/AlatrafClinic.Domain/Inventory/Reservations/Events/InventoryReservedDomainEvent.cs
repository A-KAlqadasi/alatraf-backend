using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Inventory.Reservations.Events;

public sealed record InventoryReservedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public decimal TotalAmount { get; }

    public InventoryReservedDomainEvent(int saleId, decimal totalAmount)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
    }
}
