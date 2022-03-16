namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class PermissionItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public List<PermissionItem> Children { get; set; } = new();
}
