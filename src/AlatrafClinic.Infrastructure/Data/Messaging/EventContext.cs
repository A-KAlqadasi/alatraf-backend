using AlatrafClinic.Application.Common.Events;

namespace AlatrafClinic.Infrastructure.Data.Messaging;

// Legacy placeholder; prefer Application.Common.Events.EventContext
internal sealed class InfrastructureEventContext : IEventContext
{
    public Guid? CurrentMessageId { get; set; }
}
