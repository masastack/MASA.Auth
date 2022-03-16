using Masa.Auth.ApiGateways.Caller.Response.Permissions;

namespace Masa.Auth.ApiGateways.Caller.Request.Permissions;

public class UpdateRoleRequest
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

    public UpdateRoleRequest(Guid roleId, string name, string code, int limit, string description, bool enabled, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
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

    public static implicit operator UpdateRoleRequest(RoleDetailResponse role)
    {
        return new UpdateRoleRequest(role.RoleId, role.Name, role.Code, role.Limit, role.Description, role.Enabled, role.RolePermissions, role.ChildRoles, role.Users);
    }
}

