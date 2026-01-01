using System.Threading;

using AlatrafClinic.Application.Common.Events;

namespace AlatrafClinic.Infrastructure.Eventing;

public sealed class EventContext : IEventContext
{
    private static readonly AsyncLocal<Guid?> _current = new();
    public Guid? CurrentMessageId
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}
