namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record LdapUpsertCommand(LdapDetailDto LDAPDetailDto) : Command
{
}
