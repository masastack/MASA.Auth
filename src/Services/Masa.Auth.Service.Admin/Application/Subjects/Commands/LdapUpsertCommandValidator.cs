namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class LdapUpsertCommandValidator : AbstractValidator<LdapUpsertCommand>
{
    public LdapUpsertCommandValidator()
    {
        RuleFor(command => command.LDAPDetailDto).SetValidator(new LDAPDetailValidator());
    }
}
