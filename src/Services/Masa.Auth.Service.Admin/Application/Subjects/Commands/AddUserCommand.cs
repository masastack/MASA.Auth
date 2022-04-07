namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record AddUserCommand(AddUserDto User) : Command
{
    public Guid UserId { get; set; }
}
