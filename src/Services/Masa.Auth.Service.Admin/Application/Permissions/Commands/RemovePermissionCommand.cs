namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public record RemovePermissionCommand(Guid PermissionId) : Command;
