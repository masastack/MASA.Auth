namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateUserClaimValidator : AbstractValidator<UpdateUserClaimDto>
{
    public UpdateUserClaimValidator()
    {
        RuleFor(UserClaim => UserClaim.Name).Required();
    }
}

