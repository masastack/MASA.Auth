namespace Masa.Auth.Service.Application.Subjects.Commands;

public record CreateStaffCommand(string JobNumber, string Positoon, StaffTypes StaffType, bool Enabled = true)
    : Command
{
    public AddUserCommand CreateUserCommand { get; set; } = null!;
}
