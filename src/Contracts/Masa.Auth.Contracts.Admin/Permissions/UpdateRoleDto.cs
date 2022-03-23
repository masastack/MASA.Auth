namespace Masa.Auth.Contracts.Admin.Permissions;

public class UpdateRoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> RolePermissions { get; set; }

    public List<Guid> ChildRoles { get; set; }

    public List<Guid> Users { get; set; }

    public UpdateRoleDto(Guid id, string name, string description, bool enabled, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
    {
        Id = id;
        Name = name;
        Description = description;
        Enabled = enabled;
        RolePermissions = rolePermissions;
        ChildRoles = childRoles;
        Users = users;
    }

    public static implicit operator UpdateRoleDto(RoleDetailDto role)
    {
        return new UpdateRoleDto(role.Id, role.Name, role.Description, role.Enabled, role.Permissions, role.ChildrenRoles, role.Users);
    }
}

