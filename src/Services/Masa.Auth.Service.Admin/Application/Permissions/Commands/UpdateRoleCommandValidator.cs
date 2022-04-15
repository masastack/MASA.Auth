namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(command => command.Role).SetValidator(new UpdateRoleValidator());
    }
}
