// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore;

public static class DbContextDetachExtensions
{
    /// <summary>
    /// After a failed SaveChanges the context still tracks the unsaved graph (Added) or the
    /// conflicting changes (Modified); detach the pending changes of the given users so that
    /// following operations are not polluted by the failed batch.
    /// Covered roots: User, Staff and ThirdPartyUser (the aggregates of the user insert/update
    /// flows). Owned types (e.g. User.Address / Staff.Address) are cascaded by EF Core when the
    /// owner is detached, with one exception handled below: an owned instance that was REPLACED
    /// (e.g. <c>user.UpdateBasicInfo(..., new AddressValue())</c>) leaves the old instance tracked as
    /// Deleted and EF does not cascade to it. Left behind, it nulls the NOT NULL owned columns of
    /// that row on the next SaveChanges (owned types share the owner's table) and fails it, so the
    /// Deleted owned entries of the detached owners are detached as well. Pending changes of OTHER
    /// users in the same scoped context must survive.
    /// </summary>
    public static void DetachPendingUserGraphs(this DbContext dbContext, IEnumerable<Guid> userIds)
    {
        var idSet = userIds.ToHashSet();
        var entries = dbContext.ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            if (entry.State is not (Microsoft.EntityFrameworkCore.EntityState.Added or Microsoft.EntityFrameworkCore.EntityState.Modified))
                continue;

            var belongsToTargetUsers =
                (entry.Entity is User user && idSet.Contains(user.Id)) ||
                (entry.Entity is Staff staff && idSet.Contains(staff.UserId)) ||
                (entry.Entity is ThirdPartyUser thirdPartyUser && idSet.Contains(thirdPartyUser.UserId));

            if (belongsToTargetUsers)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

        DetachOrphanedOwnedEntries(entries, idSet);
    }

    /// <summary>
    /// Detach the Deleted owned entries (replaced owned instances) whose owner was detached above.
    /// Owners are matched by the ownership foreign key (User.Address -> UserId, Staff.Address -> StaffId, ...);
    /// an owner counts as detached when its entry is Detached now - either detached explicitly or by the cascade.
    /// </summary>
    static void DetachOrphanedOwnedEntries(
        List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries,
        HashSet<Guid> userIds)
    {
        var detachedUserIds = new HashSet<Guid>();
        var detachedStaffIds = new HashSet<Guid>();
        var detachedThirdPartyUserIds = new HashSet<Guid>();
        foreach (var entry in entries)
        {
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Detached)
                continue;

            switch (entry.Entity)
            {
                case User user when userIds.Contains(user.Id):
                    detachedUserIds.Add(user.Id);
                    break;
                case Staff staff when userIds.Contains(staff.UserId):
                    detachedStaffIds.Add(staff.Id);
                    break;
                case ThirdPartyUser thirdPartyUser when userIds.Contains(thirdPartyUser.UserId):
                    detachedThirdPartyUserIds.Add(thirdPartyUser.Id);
                    break;
            }
        }

        foreach (var entry in entries)
        {
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                continue;

            var ownership = entry.Metadata.FindOwnership();
            if (ownership == null || ownership.Properties.Count != 1)
                continue;

            if (entry.Property(ownership.Properties[0].Name).CurrentValue is not Guid ownerId)
                continue;

            var ownerType = ownership.PrincipalEntityType.ClrType;
            var ownerDetached =
                (ownerType == typeof(User) && detachedUserIds.Contains(ownerId)) ||
                (ownerType == typeof(Staff) && detachedStaffIds.Contains(ownerId)) ||
                (ownerType == typeof(ThirdPartyUser) && detachedThirdPartyUserIds.Contains(ownerId));

            if (ownerDetached)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }
    }
}
