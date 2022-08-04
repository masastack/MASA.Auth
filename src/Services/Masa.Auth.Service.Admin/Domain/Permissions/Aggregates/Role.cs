// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Role : FullAggregateRoot<Guid, Guid>
{
    private List<RolePermission> _permissions = new();
    private List<RoleRelation> _childrenRoles = new();
    private List<RoleRelation> _parentRoles = new();
    private List<UserRole> _users = new();
    private List<TeamRole> _teams = new();
    private User? _createUser;
    private User? _modifyUser;
    private int _limit;

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool Enabled { get; private set; }

    public int Limit
    {
        get => _limit;
        set
        {
            if (value < 0)
                throw new UserFriendlyException("This operation cannot be completed due to role Limit restrictions");

            _limit = value;
        }
    }

    public int AvailableQuantity { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    public IReadOnlyCollection<RoleRelation> ChildrenRoles => _childrenRoles;

    public IReadOnlyCollection<RoleRelation> ParentRoles => _parentRoles;

    public IReadOnlyCollection<UserRole> Users => _users;

    public IReadOnlyCollection<TeamRole> Teams => _teams;

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

    public Role()
    {
        Name = "";
        Description = "";
    }

    public Role(string name, string description) : this(name, description, true, 1)
    {
    }

    public Role(string name, string description, bool enabled, int limit)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Limit = limit;
        AvailableQuantity = Limit;
    }

    public static implicit operator RoleDetailDto(Role role)
    {
        return new(role.Id, role.Name, role.Description, role.Enabled, role.Limit,
            role.Permissions.Select(spr => (SubjectPermissionRelationDto)spr).ToList(),
            role.ParentRoles.Select(r => r.ParentId).ToList(),
            role.ChildrenRoles.Select(r => r.RoleId).ToList(),
            role.Users.Select(u => new UserSelectDto(u.Id, u.User.Name, u.User.Name, u.User.Account, u.User.PhoneNumber, u.User.Email, u.User.Avatar)).ToList(),
            role.Teams.Select(t => t.TeamId).ToList(),
            role.CreationTime, role.ModificationTime, role.CreateUser?.Name ?? "", role.ModifyUser?.Name ?? "", role.AvailableQuantity);
    }

    public void BindChildrenRoles(List<Guid> childrenRoles)
    {
        _childrenRoles = _childrenRoles.MergeBy(
            childrenRoles.Select(roleId => new RoleRelation(roleId, default)),
            item => item.RoleId);
    }

    public void BindPermissions(List<SubjectPermissionRelationDto> permissions)
    {
        _permissions = _permissions.MergeBy(
            permissions.Select(spr => new RolePermission(spr.PermissionId, spr.Effect)),
            item => item.PermissionId,
            (oldValue, newValue) =>
            {
                oldValue.Update(newValue.Effect);
                return oldValue;
            });
    }

    public void Update(string name, string description, bool enabled, int limit)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Limit = limit;
    }

    public void UpdateAvailableQuantity(int availableQuantity)
    {
        AvailableQuantity = availableQuantity;
    }
}
