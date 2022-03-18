namespace Masa.Auth.Contracts.Admin.Permissions;

public class AddRoleDto
{
    public string Name { get; set; }

    public string Code { get; set; }

    public int Limit { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> RolePermissions { get; set; }

    public List<Guid> ChildRoles { get; set; }

    public List<Guid> Users { get; set; }

    public AddRoleDto(string name, string code, int limit, string description, bool enabled, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
    {
        Name = name;
        Code = code;
        Limit = limit;
        Description = description;
        Enabled = enabled;
        RolePermissions = rolePermissions;
        ChildRoles = childRoles;
        Users = users;
    }

    public static implicit operator AddRoleDto(RoleDetailDto role)
    {
        return new AddRoleDto(role.Name, role.Code, role.Limit, role.Description, role.Enabled, role.Permissions, role.ChildrenRoles, role.Users);
    }
}

