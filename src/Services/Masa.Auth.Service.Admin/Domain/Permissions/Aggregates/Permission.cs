// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Permission : FullAggregateRoot<Guid, Guid>
{
    public string SystemId { get; set; }

    public string AppId { get; private set; }

    public string Name { get; private set; }

    public string Code { get; private set; }

    public Guid ParentId { get; set; }

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public PermissionTypes Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<Permission> childPermissions = new();

    public IReadOnlyCollection<Permission> ChildPermissions => childPermissions;

    private List<PermissionRelation> childPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> ChildPermissionRelations => childPermissionRelations;

    private List<Permission> parentPermissions = new();

    public IReadOnlyCollection<Permission> ParentPermissions => parentPermissions;

    private List<PermissionRelation> parentPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> ParentPermissionRelations => parentPermissionRelations;

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<UserPermission> userPermissions = new();

    public IReadOnlyCollection<UserPermission> UserPermissions => userPermissions;

    private List<TeamPermission> teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => teamPermissions;

    public Permission(string systemId, string appId, string name, string code, string url,
        string icon, PermissionTypes type, string description, bool enabled)
    {
        SystemId = systemId;
        AppId = appId;
        Name = name;
        Code = code;
        Url = url;
        Icon = icon;
        Type = type;
        Description = description;
        Enabled = enabled;
    }

    public void DeleteCheck()
    {
        if (rolePermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,some role used!");
        }
        if (userPermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,some user used!");
        }
        if (parentPermissionRelations.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because relation by other permisson!");
        }
    }

    public void BindApiPermission(params Guid[] childrenId)
    {
        if (Type == PermissionTypes.Api && childrenId.Any())
        {
            throw new UserFriendlyException("the permission of api type can`t bind api permission");
        }
        childPermissionRelations.Clear();
        foreach (var childId in childrenId)
        {
            childPermissionRelations.Add(new PermissionRelation(Id, childId));
        }
    }

    public void SetParent(Guid parentId)
    {
        if (Type == PermissionTypes.Api && parentId != Guid.Empty)
        {
            throw new UserFriendlyException("the permission of api type can`t set parent");
        }
        ParentId = parentId;
    }

    public void Update(string appId, string name, string code, string url,
        string icon, PermissionTypes type, string description, bool enabled)
    {
        appId.ThrowIfEmpty();
        name.ThrowIfEmpty();
        code.ThrowIfEmpty();
        AppId = appId;
        Name = name;
        Code = code;
        Url = url;
        Icon = icon;
        Type = type;
        Description = description;
        Enabled = enabled;
    }
}