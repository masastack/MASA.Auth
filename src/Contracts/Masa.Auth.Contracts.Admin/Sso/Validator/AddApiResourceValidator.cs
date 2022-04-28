namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddApiResourceValidator : AbstractValidator<AddApiResourceDto>
{
    public AddApiResourceValidator()
    {
        RuleFor(apiResource => apiResource.Name).Required();
    }
}

