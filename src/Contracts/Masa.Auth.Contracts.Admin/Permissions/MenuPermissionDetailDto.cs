namespace Masa.Auth.Contracts.Admin.Permissions;

public class MenuPermissionDetailDto : ApiPermissionDetailDto
{
    public bool Enabled { get; set; }

    public Guid ParentId { get; set; }

    public List<RoleSelectDto> Roles { get; set; } = new();

    public List<UserSelectDto> Users { get; set; } = new();

    public List<TeamSelectDto> Teams { get; set; } = new();

    public List<Guid> ApiPermissions { get; set; } = new();
}