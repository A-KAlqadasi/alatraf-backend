namespace AlatrafClinic.Infrastructure.Data.Idempotency;

public sealed class IdempotencyKey
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = default!;
    public string Route { get; set; } = default!;
    public string RequestHash { get; set; } = default!;
    public string Status { get; set; } = default!; // InProgress | Completed | Failed
    public string? ResponseBody { get; set; }
    public int? ResponseStatusCode { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
}
