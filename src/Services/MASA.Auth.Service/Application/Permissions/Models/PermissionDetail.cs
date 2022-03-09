namespace Masa.Auth.Service.Application.Permissions.Models;

public class PermissionDetail
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
}
