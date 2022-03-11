namespace Masa.Auth.Service.Application.Subjects.Commands;

public record CreateStaffCommand(string JobNumber, string Positoon, bool Enabled = true, Guid UserId = default(Guid))
    : Command
{
    public CreateUserCommand CreateUserCommand { get; set; } = null!;
}
