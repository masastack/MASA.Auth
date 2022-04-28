namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddUserClaimValidator : AbstractValidator<AddUserClaimDto>
{
    public AddUserClaimValidator()
    {
        RuleFor(UserClaim => UserClaim.Name).Required();
    }
}

