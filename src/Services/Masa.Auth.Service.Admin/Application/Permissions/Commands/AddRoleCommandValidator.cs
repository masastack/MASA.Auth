namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleCommandValidator()
    {
        RuleFor(command => command.Role).SetValidator(new AddRoleValidator());
    }
}
