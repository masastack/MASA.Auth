namespace Masa.Auth.Service.Application.Permissions.Commands;

public record AddRoleCommand(string Name, string Description, bool Enabled,List<Guid> ChildrenRoles,List<Guid> Permissions) : Command
{

}
