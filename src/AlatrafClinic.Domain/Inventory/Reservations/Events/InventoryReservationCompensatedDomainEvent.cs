// Domain/Common/DomainEvents/InventoryReservationCompensatedDomainEvent.cs
using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Inventory.Reservations
{
    public sealed record InventoryReservationCompensatedDomainEvent : DomainEvent
    {
        public Guid ReservationId { get; }
        public Guid? SagaId { get; }

        public InventoryReservationCompensatedDomainEvent(Guid reservationId, Guid? sagaId)
        {
            ReservationId = reservationId;
            SagaId = sagaId;
        }
    }
}

