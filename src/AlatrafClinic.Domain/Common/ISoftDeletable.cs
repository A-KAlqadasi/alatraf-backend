namespace AlatrafClinic.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAtUtc { get; }
    string? DeletedBy { get; }

    void SoftDelete(string? deletedBy, DateTimeOffset deletedAtUtc);
}