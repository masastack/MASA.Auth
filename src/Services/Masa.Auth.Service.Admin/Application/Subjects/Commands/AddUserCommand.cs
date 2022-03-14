namespace Masa.Auth.Service.Application.Subjects.Commands;

public record AddUserCommand(string Name, string DisplayName, string Avatar, string IDCard, string Account,
    string Password, string CompanyName, string PhoneNumber, string Email, bool Enabled, AddressValue householdAddress, AddressValue residentialAddress) : Command
{

}
