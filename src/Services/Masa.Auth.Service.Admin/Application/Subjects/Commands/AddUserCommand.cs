namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record AddUserCommand(string Name, string DisplayName, string Avatar, string IdCard, string Account,
    string Password, string CompanyName, string PhoneNumber, string Email, bool Enabled, AddressValue householdAddress, AddressValue residentialAddress) : Command
{

}
