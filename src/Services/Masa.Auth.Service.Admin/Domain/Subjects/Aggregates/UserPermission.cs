// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserPermission : AuditEntity<Guid, Guid>, ISoftDelete
{
    private User? _user;
    private Permission? _permission;

    public bool IsDeleted { get; private set; }

    public User User
    {
        get => _user ?? throw new UserFriendlyException("Failed to get user data");
        set => _user = value;
    }

    public Permission Permission
    {
        get => _permission ?? throw new UserFriendlyException("Failed to get permission data");
        set => _permission = value;
    }

    public Guid UserId { get; private set; }

    public Guid PermissionId { get; private set; }

    public bool Effect { get; private set; }

    public UserPermission(Guid userId, Guid permissionId, bool effect)
    {
        UserId = userId;
        PermissionId = permissionId;
        Effect = effect;
    }

    public UserPermission(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }
}
