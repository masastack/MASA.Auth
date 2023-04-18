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

    public Guid? ParentId { get; private set; }

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public int Order { get; set; }

    public PermissionTypes Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<Permission> _affiliationPermissions = new();

    public IReadOnlyCollection<Permission> AffiliationPermissions => _affiliationPermissions;

    private List<PermissionRelation> _affiliationPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> AffiliationPermissionRelations => _affiliationPermissionRelations;

    private List<Permission> _leadingPermissions = new();

    public IReadOnlyCollection<Permission> LeadingPermissions => _leadingPermissions;

    private List<PermissionRelation> _leadingPermissionRelations = new();

    public IReadOnlyCollection<PermissionRelation> LeadingPermissionRelations => _leadingPermissionRelations;

    private List<UserPermission> _userPermissions = new();

    public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;

    private List<RolePermission> _rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private List<TeamPermission> _teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => _teamPermissions;

    public Permission? Parent { get; set; }

    private List<Permission> _children = new();

    public IReadOnlyCollection<Permission> Children => _children;

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

    public Guid GetParentId()
    {
        return ParentId.HasValue ? ParentId.Value : Guid.Empty;
    }

    public void DeleteCheck()
    {
        if (RolePermissions.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_DELETE_ROLE_USED_ERROR);
        }
        if (UserPermissions.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_DELETE_USER_USED_ERROR);
        }
        if (_leadingPermissionRelations.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_DELETERELATION_ERROR);
        }
    }

    public void BindApiPermission(params Guid[] childrenIds)
    {
        if (Type == PermissionTypes.Api && childrenIds.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_API_BIND_ERROR);
        }
        _affiliationPermissionRelations = _affiliationPermissionRelations.MergeBy(
            childrenIds.Select(childrenId => new PermissionRelation(Id, childrenId)),
            item => item.AffiliationPermissionId);
    }

    public void SetParent(Guid parentId)
    {
        if (Type == PermissionTypes.Api && parentId != Guid.Empty)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_API_PARENT_ERROR);
        }
        ParentId = parentId == Guid.Empty ? null : parentId;
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