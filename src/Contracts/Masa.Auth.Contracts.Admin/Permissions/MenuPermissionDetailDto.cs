namespace Masa.Auth.Contracts.Admin.Permissions;

public class MenuPermissionDetailDto : PermissionDetailDto
{
    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<RoleSelectDto> Roles { get; set; } = new();

    public List<UserSelectDto> Users { get; set; } = new();

    public List<TeamSelectDto> Teams { get; set; } = new();

    public List<Guid> ApiPermissions { get; set; } = new();
}