namespace Masa.Auth.Contracts.Admin.Permissions;

public class PermissionDto
{
    public Guid Id { get; set; }

    public PermissionTypes Type { get; set; }

    public string Name { get; set; } = "";
}
