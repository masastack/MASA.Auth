namespace Masa.Auth.ApiGateways.Caller.Response.Permissions;

public class RoleDetailResponse
{
    public Guid RoleId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public int Limit { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> RolePermissions { get; set; }

    public List<Guid> ChildRoles { get; set; }

    public List<Guid> Users { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public static RoleDetailResponse Default = new(default, "", "", 0, "", true, new(), new(), new(), default, default, "", "");

    public RoleDetailResponse(Guid roleId, string name, string code, int limit, string description, bool enabled, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        RoleId = roleId;
        Name = name;
        Code = code;
        Limit = limit;
        Description = description;
        Enabled = enabled;
        RolePermissions = rolePermissions;
        ChildRoles = childRoles;
        Users = users;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


