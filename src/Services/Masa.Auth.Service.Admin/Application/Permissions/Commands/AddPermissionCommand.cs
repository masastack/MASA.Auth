namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public record AddPermissionCommand(PermissionDetailDto PermissionDetail) : Command
{
    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<Guid> ApiPermissions { get; set; } = new();
}
