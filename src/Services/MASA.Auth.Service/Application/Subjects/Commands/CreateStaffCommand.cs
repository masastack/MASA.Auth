namespace MASA.Auth.Service.Application.Subjects.Commands;

public record CreateStaffCommand(string Name, string DisplayName, string Avatar, string IDCard, string Account,
    string Password, string CompanyName, string PhoneNumber, string Email, string JobNumber, Guid UserId = default(Guid))
    : Command
{
}
