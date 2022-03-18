namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record RemoveUserCommand(Guid UserId) : Command
{
}
