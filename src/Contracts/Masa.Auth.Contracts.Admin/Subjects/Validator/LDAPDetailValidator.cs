namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class LDAPDetailValidator : AbstractValidator<LdapDetailDto>
{
    public LDAPDetailValidator()
    {
        RuleFor(l => l.ServerPort.ToString()).Port();
        RuleFor(l => l.ServerAddress).Required();
        RuleFor(l => l.RootUserDn).Required();
        RuleFor(l => l.RootUserPassword).Required();
        RuleFor(l => l.BaseDn).MinLength(3);
    }
}
