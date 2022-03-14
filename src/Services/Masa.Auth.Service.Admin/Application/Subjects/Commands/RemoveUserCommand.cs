namespace Masa.Auth.Service.Application.Subjects.Commands;

public record RemoveUserCommand(Guid UserId) : Command
{
}
