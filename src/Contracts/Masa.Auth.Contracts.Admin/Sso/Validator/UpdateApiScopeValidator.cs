namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateApiScopeValidator : AbstractValidator<UpdateApiScopeDto>
{
    public UpdateApiScopeValidator()
    {
        RuleFor(apiScope => apiScope.Name).Required();
    }
}

