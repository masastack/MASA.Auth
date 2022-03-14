namespace Masa.Auth.Service.Application.Subjects.Commands;

public record CreateStaffCommand(string JobNumber, string Positoon, bool Enabled = true)
    : Command
{
    public AddUserCommand CreateUserCommand { get; set; } = null!;
}
