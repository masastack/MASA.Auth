namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public int Limit { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<Guid> Users { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public static RoleDetailDto Default = new(default, "", "", 0, "", true, new(), new(), new(), default, default, "", "");

    public RoleDetailDto(Guid id, string name, string code, int limit, string description, bool enabled, List<Guid> permissions, List<Guid> childrenRoles, List<Guid> users, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        Id = id;
        Name = name;
        Code = code;
        Limit = limit;
        Description = description;
        Enabled = enabled;
        Permissions = permissions;
        ChildrenRoles = childrenRoles;
        Users = users;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


