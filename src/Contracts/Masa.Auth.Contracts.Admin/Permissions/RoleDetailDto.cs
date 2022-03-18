namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDetailDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<Guid> Permissions { get; set; }

    public RoleDetailDto(string name, string description, bool enabled, List<Guid> childrenRoles, List<Guid> permissions)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        ChildrenRoles = childrenRoles;
        Permissions = permissions;
    }
}