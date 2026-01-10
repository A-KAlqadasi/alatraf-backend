namespace AlatrafClinic.Application.Common.Interfaces.Messaging;

public interface IInbox
{
    Task<bool> TryProcessAsync(
        Guid messageId,
        string handlerName,
        CancellationToken ct);
}
