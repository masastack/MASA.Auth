namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDetailDto : RoleDto
{
    public List<Guid> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<Guid> Users { get; set; }

    public int QuantityAvailable { get; set; }

    public RoleDetailDto() : base()
    {
        Permissions = new();
        ChildrenRoles = new();
        Users = new();
    }

    public RoleDetailDto(Guid id, string name, string description, bool enabled, int limit, List<Guid> permissions, List<Guid> childrenRoles, List<Guid> users, DateTime creationTime, DateTime? modificationTime, string creator, string modifier, int quantityAvailable) : base(id, name, limit, description, enabled, creationTime, modificationTime, creator, modifier)
    {
        Permissions = permissions;
        ChildrenRoles = childrenRoles;
        Users = users;
        QuantityAvailable = quantityAvailable;
    }
}


