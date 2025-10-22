namespace AlatrafClinic.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>
{
    protected AuditableEntity()
    { }

    protected AuditableEntity(TId id)
        : base(id)
    {
    }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModifiedUtc { get; set; }

    public string? LastModifiedBy { get; set; }
}