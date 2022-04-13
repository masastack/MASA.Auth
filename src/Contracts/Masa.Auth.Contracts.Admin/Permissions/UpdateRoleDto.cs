namespace Masa.Auth.Contracts.Admin.Permissions;

public class UpdateRoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public int Limit { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<Guid> Users { get; set; }

    public UpdateRoleDto()
    {
        Name = "";
        Description = "";
        Permissions = new();
        ChildrenRoles = new();
        Users = new();
    }

    public UpdateRoleDto(Guid id, string name, string description, bool enabled, int limit,List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
    {
        Id = id;
        Name = name;
        Description = description;
        Enabled = enabled;
        Permissions = rolePermissions;
        ChildrenRoles = childRoles;
        Users = users;
        Limit = limit;
    }

    public static implicit operator UpdateRoleDto(RoleDetailDto role)
    {
        return new UpdateRoleDto(role.Id, role.Name, role.Description, role.Enabled, role.Limit,role.Permissions, role.ChildrenRoles, role.Users);
    }
}

