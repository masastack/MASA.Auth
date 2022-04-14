﻿using Masa.Utils.Data.EntityFrameworkCore.Filters;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Masa.Auth.Service.Admin.Infrastructure;

#warning Temporary 
public class SoftDeleteSaveChangesFilter<TDbContext> : ISaveChangesFilter where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private readonly MasaDbContextOptions _masaDbContextOptions;

    public SoftDeleteSaveChangesFilter(MasaDbContextOptions masaDbContextOptions, TDbContext dbContext)
    {
        _masaDbContextOptions = masaDbContextOptions;
        _context = dbContext;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        if (!_masaDbContextOptions.EnableSoftDelete)
            return;

        changeTracker.DetectChanges();
        foreach (var entity in changeTracker.Entries().Where(entry => entry.State == EntityState.Deleted))
        {
            if (entity.Entity is ISoftDelete)
            {
                HandleNavigationEntry(entity.Navigations.Where(n => !((IReadOnlyNavigation)n.Metadata).IsOnDependent));

                entity.State = EntityState.Modified;
                entity.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
            }
        }
    }

    protected virtual void HandleNavigationEntry(IEnumerable<NavigationEntry> navigationEntries)
    {
        foreach (var navigationEntry in navigationEntries)
        {
            if (navigationEntry is CollectionEntry collectionEntry)
            {
                foreach (var dependentEntry in collectionEntry.CurrentValue ?? new List<object>())
                {
                    HandleDependent(dependentEntry);
                }
            }
            else
            {
                var dependentEntry = navigationEntry.CurrentValue;
                if (dependentEntry != null)
                {
                    HandleDependent(dependentEntry);
                }
            }
        }
    }

    protected virtual void HandleDependent(object dependentEntry)
    {
        var entityEntry = _context.Entry(dependentEntry);
        entityEntry.State = EntityState.Modified;

        if (entityEntry.Entity is ISoftDelete)
            entityEntry.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
    }
}
