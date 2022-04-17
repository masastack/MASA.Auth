namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class UpsertDepartmentValidator : AbstractValidator<UpsertDepartmentDto>
{
    public UpsertDepartmentValidator()
    {
        RuleFor(d => d).NotNull().WithMessage($"Parameter error");
        RuleFor(d => d.Name).Must(name => !string.IsNullOrWhiteSpace(name) && name.Length <= 20)
            .WithMessage("Department Name can`t null and length must be less than 20.");
        RuleFor(d => d.Description).Must(description => description.Length <= 255)
            .WithMessage("Department Description length must be less than 255.");
    }
}
