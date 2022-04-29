namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class AddApiScopeCommandValidator : AbstractValidator<AddApiScopeCommand>
{
    public AddApiScopeCommandValidator()
    {
        RuleFor(command => command.ApiScope).SetValidator(new AddApiScopeValidator());
    }
}
