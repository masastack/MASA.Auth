namespace Masa.Auth.Contracts.Admin.Permissions;

public class AppPermissionDto
{
    public string AppId { get; set; } = "";

    public string AppName { get; set; } = "";

    public List<PermissionDto> Permissions { get; set; } = new();
}
