namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateApiResourceValidator : AbstractValidator<UpdateApiResourceDto>
{
    public UpdateApiResourceValidator()
    {
        RuleFor(apiResource => apiResource.Name).Required();
    }
}

