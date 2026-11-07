using DonationHub.Application.Interfaces.Services;
using DonationHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    private readonly ICurrentUserService _currentUserService;

    public SoftDeleteInterceptor(TimeProvider timeProvider, ICurrentUserService currentUserService)
    {
        _timeProvider = timeProvider;
        _currentUserService = currentUserService;
    }
    private void ApplySoftDelete(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var userId = _currentUserService.UserId;

        var entries = context.ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.MarkAsDeleted(utcNow, userId);
        }
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
