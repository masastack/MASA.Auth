namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public record AddPermissionCommand(int SystemId, string AppId, Guid ParentId, string Name,
    string Code, string Icon, PermissionType Type, string Url, string Description, bool Enabled) : Command
{
    public List<Guid> ApiPermissionIds { get; set; } = new();
}
