namespace Masa.Auth.Service.Application.Permissions.Commands;

public record RemoveRoleCommand(Guid RoleId) : Command;
