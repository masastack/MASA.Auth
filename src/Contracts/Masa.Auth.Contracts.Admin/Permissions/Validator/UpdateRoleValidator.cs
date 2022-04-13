namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

internal class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(role => role.Name).Required().ChineseLetterNumber().MaxLength(20);
        RuleFor(role => role.Description).MaxLength(50);
        RuleFor(role => role.Limit).GreaterThan(0);
    }
}

