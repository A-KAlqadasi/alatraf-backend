using AlatrafClinic.Application.Common.Interfaces.Messaging;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Messaging;

public sealed class NoopMessagePublisher : IMessagePublisher
{
    private readonly ILogger<NoopMessagePublisher> _logger;

    public NoopMessagePublisher(ILogger<NoopMessagePublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(object @event, CancellationToken ct = default)
    {
        _logger.LogInformation("NoopMessagePublisher - would publish event {EventType}", @event?.GetType().FullName);
        return Task.CompletedTask;
    }
}
