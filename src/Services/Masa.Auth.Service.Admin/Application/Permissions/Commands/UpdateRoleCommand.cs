namespace Masa.Auth.Service.Application.Permissions.Commands;

public record UpdateRoleCommand(Guid RoleId, string Name, string Description, bool Enabled, List<Guid> ChildrenRoles, List<Guid> Permissions) : Command
{
}
