namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class AddIdentityResourceCommandValidator : AbstractValidator<AddIdentityResourceCommand>
{
    public AddIdentityResourceCommandValidator()
    {
        RuleFor(command => command.IdentityResource).SetValidator(new AddIdentityResourceValidator());
    }
}
