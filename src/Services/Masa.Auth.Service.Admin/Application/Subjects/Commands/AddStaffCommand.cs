namespace Masa.Auth.Service.Application.Subjects.Commands;

public record AddStaffCommand(string JobNumber, string Positoon, Infrastructure.Enums.MemberTypes StaffType, bool Enabled = true)
    : Command
{
    public AddUserCommand CreateUserCommand { get; set; } = null!;
}
