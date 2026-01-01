namespace AlatrafClinic.Application.Common.Interfaces.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(object @event, CancellationToken ct = default);
}
