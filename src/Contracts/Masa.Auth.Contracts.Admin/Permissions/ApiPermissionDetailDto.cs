namespace Masa.Auth.Contracts.Admin.Permissions;

public class ApiPermissionDetailDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public PermissionTypes Type { get; set; } = PermissionTypes.Api;

    public string Description { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}