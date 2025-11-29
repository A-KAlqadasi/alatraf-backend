using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AlatrafClinic.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(IUser user, TimeProvider dateTime) : SaveChangesInterceptor
{
    private readonly IUser _user = user;
    private readonly TimeProvider _dateTime = dateTime;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = _dateTime.GetUtcNow();

        // 1) Audit create/update
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy    = _user.Id;
                entry.Entity.CreatedAtUtc = utcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy  = _user.Id;
                entry.Entity.LastModifiedUtc = utcNow;
            }

            // handle owned auditable entities
            foreach (var reference in entry.References)
            {
                if (reference.TargetEntry?.Entity is IAuditableEntity owned &&
                    reference.TargetEntry.State is EntityState.Added or EntityState.Modified)
                {
                    if (reference.TargetEntry.State == EntityState.Added)
                    {
                        owned.CreatedBy    = _user.Id;
                        owned.CreatedAtUtc = utcNow;
                    }

                    owned.LastModifiedBy  = _user.Id;
                    owned.LastModifiedUtc = utcNow;
                }
            }
        }

        // 2) Soft delete
        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // flip to soft delete instead of physical delete
                entry.Entity.SoftDelete(_user.Id, utcNow);
                entry.State = EntityState.Modified;
            }
        }
    }
}

public static class EntityEntryExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry?.Metadata.IsOwned() == true &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
