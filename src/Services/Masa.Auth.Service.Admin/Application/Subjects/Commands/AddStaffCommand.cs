using Masa.Auth.Service.Admin.Infrastructure.Enums;

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record AddStaffCommand(string JobNumber, string Positoon, StaffTypes StaffType, bool Enabled = true)
    : Command
{
    public AddUserCommand CreateUserCommand { get; set; } = null!;
}
