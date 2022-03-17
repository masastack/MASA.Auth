namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class AppPermissionDto
{
    public string AppId { get; set; } = "";

    public string AppName { get; set; } = "";

    public List<PermissionDto> Permissions { get; set; } = new();
}
