using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Inventory.Reservations.Events;

public sealed record InventoryReservationFailedDomainEvent : DomainEvent
{
    public int SaleId { get; }

    public InventoryReservationFailedDomainEvent(int saleId)
    {
        SaleId = saleId;
    }
}
