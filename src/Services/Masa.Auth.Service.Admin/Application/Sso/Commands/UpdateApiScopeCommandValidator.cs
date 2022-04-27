namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class UpdateApiScopeCommandValidator : AbstractValidator<UpdateApiScopeCommand>
{
    public UpdateApiScopeCommandValidator()
    {
        RuleFor(command => command.ApiScope).SetValidator(new UpdateApiScopeValidator());
    }
}
