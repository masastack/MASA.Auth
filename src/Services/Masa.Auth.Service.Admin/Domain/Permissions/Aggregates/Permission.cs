// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Permission : FullAggregateRoot<Guid, Guid>
{
    public string SystemId { get; set; }

    public string AppId { get; private set; }

    public string Name { get; private set; }

    public string Code { get; private set; }

    public string ReplenishCode => $"{AppId}.{Code}";

    public Guid ParentId { get; set; }

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public int Order { get; set; }

    public PermissionTypes Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<Permission> childPermissions = new();

    public IReadOnlyCollection<Permission> ChildPermissions => childPermissions;

    private List<PermissionRelation> _childPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> ChildPermissionRelations => _childPermissionRelations;

    private List<Permission> parentPermissions = new();

    public IReadOnlyCollection<Permission> ParentPermissions => parentPermissions;

    private List<PermissionRelation> parentPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> ParentPermissionRelations => parentPermissionRelations;

    private List<UserPermission> _userPermissions = new();

    public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;

    private List<RolePermission> _rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private List<TeamPermission> _teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => _teamPermissions;

    public Permission(Guid id, string systemId, string appId, string name, string code, string url,
        string icon, int order, PermissionTypes type) : this(systemId, appId, name, code, url, icon, order, type)
    {
        Id = id;
    }

    public Permission(string systemId, string appId, string name, string code, string url,
        string icon, int order, PermissionTypes type) : this(systemId, appId, name, code, url, icon, type, "", order, true)
    {
    }

    public Permission(string systemId, string appId, string name, string code, string url,
        string icon, PermissionTypes type, string description, int order, bool enabled)
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
        Order = order;
    }

    public void DeleteCheck()
    {
        if (RolePermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,some role used!");
        }
        if (UserPermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,some user used!");
        }
        if (parentPermissionRelations.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because relation by other permisson!");
        }
    }

    public void BindApiPermission(params Guid[] childrenIds)
    {
        if (Type == PermissionTypes.Api && childrenIds.Any())
        {
            throw new UserFriendlyException("the permission of api type can`t bind api permission");
        }
        _childPermissionRelations = _childPermissionRelations.MergeBy(
            childrenIds.Select(childrenId => new PermissionRelation(Id, childrenId)),
            item => item.ChildPermissionId);
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
        string icon, PermissionTypes type, string description, int order, bool enabled)
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
        Order = order;
        Enabled = enabled;
    }
}