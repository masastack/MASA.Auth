namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateCustomLoginValidator : AbstractValidator<UpdateCustomLoginDto>
{
    public UpdateCustomLoginValidator()
    {
        RuleFor(CustomLogin => CustomLogin.Name).Required();
    }
}

