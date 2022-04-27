namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddApiResourceValidator : AbstractValidator<ApiResourceDto>
{
    public AddApiResourceValidator()
    {
        RuleFor(apiResource => apiResource.Name).Required();
    }
}

