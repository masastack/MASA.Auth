namespace Masa.Auth.Service.Application.Permissions.Commands;

public record DeletePermissionCommand(Guid PermissionId) : Command;
