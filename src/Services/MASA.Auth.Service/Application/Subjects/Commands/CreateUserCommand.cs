namespace Masa.Auth.Service.Application.Subjects.Commands;

public record CreateUserCommand(string Name, string DisplayName, string Avatar, string IDCard, string Account,
    string Password, string CompanyName, string PhoneNumber, string Email) : Command
{

}
