namespace Masa.Auth.Service.Admin.Dto.Permissions;

public class PermissionDetailDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public PermissionType Type { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; } = "";

    public string AppId { get; set; } = "";

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public List<RoleSelectDto> RoleItems { get; set; } = new();

    public List<UserSelectDto> UserItems { get; set; } = new();

    public List<TeamSelectDto> TeamItems { get; set; } = new();
}