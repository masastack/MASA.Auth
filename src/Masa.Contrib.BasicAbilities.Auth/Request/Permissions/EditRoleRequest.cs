using Masa.Contrib.BasicAbilities.Auth.Response.Permissions;

namespace Masa.Contrib.BasicAbilities.Auth.Request.Permissions;

public class EditRoleRequest
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

    public EditRoleRequest(Guid roleId, string name, string code, int limit, string description, bool enabled, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
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
    }

    public static implicit operator EditRoleRequest(RoleDetailResponse role)
    {
        return new EditRoleRequest(role.RoleId, role.Name, role.Code, role.Limit, role.Description, role.Enabled, role.RolePermissions, role.ChildRoles, role.Users);
    }
}

