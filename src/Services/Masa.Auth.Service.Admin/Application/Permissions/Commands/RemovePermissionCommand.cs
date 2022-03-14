namespace Masa.Auth.Service.Application.Permissions.Commands;

public record RemovePermissionCommand(Guid PermissionId) : Command;
