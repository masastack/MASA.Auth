namespace Masa.Auth.Contracts.Admin.Permissions;

public class PermissionDetailDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public PermissionTypes Type { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; } = "";

    public string AppId { get; set; } = "";

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public List<RoleSelectDto> Roles { get; set; } = new();

    public List<UserSelectDto> Users { get; set; } = new();

    public List<TeamSelectDto> Teams { get; set; } = new();
}