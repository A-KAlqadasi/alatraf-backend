namespace AlatrafClinic.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>, ISoftDeletable
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
    
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
}
