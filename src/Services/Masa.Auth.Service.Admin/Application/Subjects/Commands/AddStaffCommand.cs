namespace Masa.Auth.Service.Application.Subjects.Commands;

public record AddStaffCommand(string JobNumber, string Positoon, StaffTypes StaffType, bool Enabled = true)
    : Command
{
    public AddUserCommand CreateUserCommand { get; set; } = null!;
}
