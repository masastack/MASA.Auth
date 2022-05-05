namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddCustomLoginValidator : AbstractValidator<AddCustomLoginDto>
{
    public AddCustomLoginValidator()
    {
        RuleFor(CustomLogin => CustomLogin.Name).Required();
    }
}

