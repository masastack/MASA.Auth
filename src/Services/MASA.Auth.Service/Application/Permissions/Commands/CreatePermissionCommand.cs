namespace Masa.Auth.Service.Application.Permissions.Commands;

public record CreatePermissionCommand(int SystemId, string AppId, Guid ParentId, string Name,
    string Code, string Icon, PermissionType Type, string Url, string Description, bool Enabled) : Command
{
    public List<Guid> ApiPermissionIds { get; set; } = new();
}
