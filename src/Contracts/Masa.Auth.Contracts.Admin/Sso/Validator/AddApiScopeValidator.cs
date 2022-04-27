namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddApiScopeValidator : AbstractValidator<AddApiScopeDto>
{
    public AddApiScopeValidator()
    {
        RuleFor(apiScope => apiScope.Name).Required();
    }
}

