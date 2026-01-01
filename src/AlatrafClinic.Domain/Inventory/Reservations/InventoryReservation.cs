using AlatrafClinic.Domain.Common;

public class InventoryReservation : AuditableEntity<Guid>, IAggregateRoot
{
    // SagaId is optional to keep existing data and flows working; populated when saga-aware flows call it
    public Guid? SagaId { get; private set; }
    public int SaleId { get; private set; }
    public int StoreItemUnitId { get; private set; }
    public decimal Quantity { get; private set; }
    public ReservationStatus Status { get; private set; }

    private InventoryReservation() { }

    private InventoryReservation(Guid? sagaId, int saleId, int storeItemUnitId, decimal quantity)
        : base(Guid.NewGuid())
    {
        SagaId = sagaId == Guid.Empty ? null : sagaId;
        SaleId = saleId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        Status = ReservationStatus.Reserved;
    }

    public static InventoryReservation Create(
        Guid? sagaId,
        int saleId,
        int storeItemUnitId,
        decimal quantity)
    {
        return new InventoryReservation(sagaId, saleId, storeItemUnitId, quantity);
    }

    public void Release()
    {
        Status = ReservationStatus.Released;
    }
}
