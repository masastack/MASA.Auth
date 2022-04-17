namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class AddDepartmentCommandValidator : AbstractValidator<AddDepartmentCommand>
{
    public AddDepartmentCommandValidator()
    {
        RuleFor(command => command.UpsertDepartmentDto).SetValidator(new UpsertDepartmentValidator());
    }
}

