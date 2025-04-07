// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Linq;

namespace Masa.Auth.Domain.Permissions.Aggregates;

public class Role : FullAggregateRoot<Guid, Guid>
{
    private List<RolePermission> _permissions = new();
    private List<RoleRelation> _childrenRoles = new();
    private List<RoleRelation> _parentRoles = new();
    private List<UserRole> _users = new();
    private List<TeamRole> _teams = new();
    private List<RoleClient> _clients = new();
    private readonly User? _createUser;
    private readonly User? _modifyUser;
    private string _name = "";
    private string _code = "";
    private string _description = "";
    private int _limit;
    private RoleTypes _type;

    public string Name
    {
        get => _name;
        set
        {
            _name = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(Name));
        }
    }

    public string Code
    {
        get => _code;
        set
        {
            _code = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(Code));
        }
    }

    [AllowNull]
    public string Description
    {
        get => _description;
        set => _description = value ?? "";
    }

    public bool Enabled { get; private set; }

    public int Limit
    {
        get => _limit;
        set
        {
            if (value < 0)
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_LIMIT_ERROR);

            _limit = value;
        }
    }

    public RoleTypes Type
    {
        get => _type;
        private set
        {
            _type = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(Type));
        }
    }

    public int AvailableQuantity { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    public IReadOnlyCollection<RoleRelation> ChildrenRoles => _childrenRoles;

    public IReadOnlyCollection<RoleRelation> ParentRoles => _parentRoles;

    public IReadOnlyCollection<UserRole> Users => _users;

    public IReadOnlyCollection<TeamRole> Teams => _teams;

    public IReadOnlyCollection<RoleClient> Clients => _clients;

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

    public Role(string name, string code, string? description, bool enabled, int limit, RoleTypes type)
    {
        Name = name;
        Code = code;
        Description = description;
        Enabled = enabled;
        Limit = limit;
        AvailableQuantity = Limit;
        Type = type;
    }

    public static implicit operator RoleDetailDto(Role role)
    {
        return new(role.Id, role.Name, role.Code, role.Limit, role.Type, role.Description, role.Enabled,
            role.CreationTime, role.ModificationTime, role.CreateUser?.DisplayName ?? "", role.ModifyUser?.DisplayName ?? "",
            role.Permissions.Select(rp => (SubjectPermissionRelationDto)rp).ToList(),
            role.ParentRoles.Select(r => r.ParentId).ToList(),
            role.ChildrenRoles.Select(r => r.RoleId).ToList(),
            role.Users.Select(u => new UserSelectDto(u.Id, u.User.Name, u.User.Name, u.User.Account, u.User.PhoneNumber, u.User.Email, u.User.Avatar)).ToList(),
            role.Teams.Select(t => t.TeamId).ToList(),
            role.Clients.Select(c => c.ClientId).ToList(),
            role.AvailableQuantity);
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

    public void Update(string name, string code, string? description, bool enabled, int limit, RoleTypes type)
    {
        Name = name;
        Code = code;
        Description = description;
        Enabled = enabled;
        Limit = limit;
        Type = type;
    }

    public void UpdateAvailableQuantity(int availableQuantity)
    {
        AvailableQuantity = availableQuantity;
    }

    public void AddUsers(IEnumerable<Guid> userIds)
    {
        _users = _users.MergeDistinctBy(userIds.Select(userId => new UserRole(userId, Id)), item => item.UserId).ToList();
    }

    public void RemoveUsers(IEnumerable<Guid> userIds)
    {
        _users = _users.Where(user => userIds.Any(userId => user.UserId == userId) is false)
                       .ToList();
    }

    public void BindClients(List<string> clientIds)
    {
        _clients = _clients.MergeBy(
            clientIds.Select(clientId => new RoleClient(Id, clientId)),
            item => item.ClientId);
    }

    public void AddClients(IEnumerable<string> clientIds)
    {
        foreach (var clientId in clientIds)
        {
            if (_type == RoleTypes.Client && _clients.Any())
                throw new InvalidOperationException("A client role can only be associated with one client.");

            var roleClient = new RoleClient(this.Id, clientId);
            _clients.Add(roleClient);
        }
    }

    public void RemoveClients(IEnumerable<string> clientIds)
    {
        _clients.RemoveAll(rc => clientIds.Contains(rc.ClientId));
    }
}
