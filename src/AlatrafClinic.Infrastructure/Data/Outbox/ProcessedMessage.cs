namespace AlatrafClinic.Infrastructure.Data.Outbox;

public sealed class ProcessedMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    public string HandlerName { get; set; } = default!;
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}
