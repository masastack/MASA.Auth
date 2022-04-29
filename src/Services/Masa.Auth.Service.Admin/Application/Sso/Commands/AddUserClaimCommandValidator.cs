namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class AddUserClaimCommandValidator : AbstractValidator<AddUserClaimCommand>
{
    public AddUserClaimCommandValidator()
    {
        RuleFor(command => command.UserClaim).SetValidator(new AddUserClaimValidator());
    }
}
