namespace Masa.Auth.Service.Application.Subjects.Commands;

public record DeleteUserCommand(Guid UserId) : Command
{
}
