namespace Masa.Auth.Contracts.Admin.Permissions;

public class AddRoleDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public int Limit { get; set; } = 1;

    public List<Guid> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<Guid> Users { get; set; }

    public AddRoleDto()
    {
        Name = "";
        Description = "";
        Enabled = true;
        Permissions = new();
        ChildrenRoles = new();
        Users = new();
    }

    public AddRoleDto(string name, string description, bool enabled,int limit, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Permissions = rolePermissions;
        ChildrenRoles = childRoles;
        Users = users;
        Limit = limit;
    }
}

