namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record LdapConnectTestCommand(LdapDetailDto LDAPDetailDto) : Command
{
}
