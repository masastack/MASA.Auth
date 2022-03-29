namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class AddDepartmentCommandValidator : AbstractValidator<AddDepartmentCommand>
{
    public AddDepartmentCommandValidator()
    {
        RuleFor(command => command).NotNull().WithMessage($"Parameter error");
        RuleFor(command => command.UpsertDepartmentDto.Name).Must(name => !string.IsNullOrEmpty(name) && name.Length <= 20)
            .WithMessage("Department Name can`t null and length must be less than 20.");
        RuleFor(command => command.UpsertDepartmentDto.Description).Must(description => description.Length <= 255)
            .WithMessage("Department Description length must be less than 255.");
    }
}

