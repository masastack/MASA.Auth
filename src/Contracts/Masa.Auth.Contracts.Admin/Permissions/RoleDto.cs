namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDto
{
    public Guid RoleId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public int Limit { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public RoleDto(Guid roleId, string name, string code, int limit, string description, bool enabled, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        RoleId = roleId;
        Name = name;
        Code = code;
        Limit = limit;
        Description = description;
        Enabled = enabled;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


