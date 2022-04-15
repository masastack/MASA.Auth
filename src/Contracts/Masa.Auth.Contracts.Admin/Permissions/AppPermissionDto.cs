namespace Masa.Auth.Contracts.Admin.Permissions;

public class AppPermissionDto
{
    public string AppId { get; set; } = string.Empty;

    public PermissionTypes Type { get; set; }

    public Guid PermissonId { get; set; }

    public string PermissonName { get; set; } = string.Empty;
}
