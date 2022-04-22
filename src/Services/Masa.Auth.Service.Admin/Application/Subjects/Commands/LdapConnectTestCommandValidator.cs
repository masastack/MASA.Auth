namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class LdapConnectTestCommandValidator : AbstractValidator<LdapUpsertCommand>
{
    public LdapConnectTestCommandValidator()
    {
        RuleFor(command => command.LDAPDetailDto).SetValidator(new LDAPDetailValidator());
    }
}
