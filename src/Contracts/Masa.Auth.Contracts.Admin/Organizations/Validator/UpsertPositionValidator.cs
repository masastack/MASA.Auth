namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class UpsertPositionValidator : AbstractValidator<UpsertPositionDto>
{
    public UpsertPositionValidator()
    {
        RuleFor(p => p.Name).Required();
    }
}
