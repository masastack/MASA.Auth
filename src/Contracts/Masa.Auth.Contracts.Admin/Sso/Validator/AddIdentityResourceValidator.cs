namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddIdentityResourceValidator : AbstractValidator<AddIdentityResourceDto>
{
    public AddIdentityResourceValidator()
    {
        RuleFor(user => user.Name).Required();
    }
}

