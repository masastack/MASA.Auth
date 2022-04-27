namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class AddOrUpdatePositionValidator : AbstractValidator<AddOrUpdatePositionDto>
{
    public AddOrUpdatePositionValidator()
    {
        RuleFor(p => p.Name).Required();
    }
}
