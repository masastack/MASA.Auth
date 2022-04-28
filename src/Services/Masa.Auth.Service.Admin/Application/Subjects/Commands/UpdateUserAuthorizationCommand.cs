namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record UpdateUserAuthorizationCommand(UpdateUserAuthorizationDto User) : Command
{
}
