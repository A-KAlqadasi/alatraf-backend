using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.RepairCards.Events;

public sealed class RepairCardCompleted :DomainEvent
{
    public int RepairCardId { get; set; }
}