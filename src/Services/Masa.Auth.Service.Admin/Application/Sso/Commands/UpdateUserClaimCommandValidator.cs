namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class UpdateUserClaimCommandValidator : AbstractValidator<UpdateUserClaimCommand>
{
    public UpdateUserClaimCommandValidator()
    {
        RuleFor(command => command.UserClaim).SetValidator(new UpdateUserClaimValidator());
    }
}
