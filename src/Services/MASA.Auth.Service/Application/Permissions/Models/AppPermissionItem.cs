namespace Masa.Auth.Service.Application.Permissions.Models;

public class AppPermissionItem
{
    public string AppId { get; set; } = "";

    public string AppName { get; set; } = "";

    public List<PermissionItem> Permissions { get; set; } = new();
}
