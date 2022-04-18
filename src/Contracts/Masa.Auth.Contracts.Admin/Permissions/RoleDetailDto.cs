namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDetailDto : RoleDto
{
    public List<Guid> Permissions { get; set; }

    public List<Guid> ParentRoles { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<UserSelectDto> Users { get; set; }

    public List<Guid> Teams { get; set; }

    public int AvailableQuantity { get; set; }

    public RoleDetailDto() : base()
    {
        Permissions = new();
        ParentRoles = new();
        ChildrenRoles = new();
        Users = new();
        Teams = new();
    }

    public RoleDetailDto(Guid id, string name, string description, bool enabled, int limit, List<Guid> permissions, List<Guid> parentRoles, List<Guid> childrenRoles, List<UserSelectDto> users, List<Guid> teams, DateTime creationTime, DateTime? modificationTime, string creator, string modifier, int availableQuantity) : base(id, name, limit, description, enabled, creationTime, modificationTime, creator, modifier)
    {
        Permissions = permissions;
        ParentRoles = parentRoles;
        ChildrenRoles = childrenRoles;
        Users = users;
        Teams = teams;
        AvailableQuantity = availableQuantity;
    }
}


