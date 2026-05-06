using Domain.Common;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Service.Interface;

namespace Web.Interceptor;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;

    public AuditInterceptor(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context!;
        var entries = context.ChangeTracker
            .Entries<BaseAuditableEntity<EventsAppUser>>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var user = _currentUser.GetUserId() ?? "system";

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = user;
                entry.Entity.DateCreated = now;
                entry.Entity.LastModifiedById = user;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedById = user;
                entry.Entity.DateLastModified = now;
            }
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context!;
        var entries = context.ChangeTracker
            .Entries<BaseAuditableEntity<EventsAppUser>>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var user = _currentUser.GetUserId() ?? "system";

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = user;
                entry.Entity.DateCreated = now;
                entry.Entity.LastModifiedById = user;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedById = user;
                entry.Entity.DateLastModified = now;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}