namespace MASA.Auth.Service.Application.Organizations.Commands;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(command => command)
            .NotNull().WithMessage($"Parameter error");
    }
}

