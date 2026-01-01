using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.RepairCards.Events;

public sealed record RepairCardCompleted : DomainEvent
{
    public int RepairCardId { get; set; }
}