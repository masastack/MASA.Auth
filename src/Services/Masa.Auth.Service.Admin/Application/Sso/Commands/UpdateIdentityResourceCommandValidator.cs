namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class UpdateIdentityResourceCommandValidator : AbstractValidator<UpdateIdentityResourceCommand>
{
    public UpdateIdentityResourceCommandValidator()
    {
        RuleFor(command => command.IdentityResource).SetValidator(new UpdateIdentityResourceValidator());
    }
}
